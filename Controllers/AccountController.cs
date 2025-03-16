using EmployeeLeave.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using EmployeeLeave.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using EmployeeLeave.Data.Table;
using Microsoft.EntityFrameworkCore;
using EmployeeLeave.Data;

namespace EmployeeLeave.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserStore<ApplicationUser> userStore,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
             ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("The default UI requires a user store with email support.");

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return View(model);
            }

            var user = new ApplicationUser();
            await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Employee");
                var profile = new Profile
                {
                    Name = model.Name,
                    EmployeeId = Guid.Parse(await _userManager.GetUserIdAsync(user)), 
                    Department = null 
                };

                _context.profiles.Add(profile);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User registered and profile created.");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

            // Clear any existing external cookies before login
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["ExternalLogins"] = externalLogins;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                var externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                ViewData["ExternalLogins"] = externalLogins;
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return LocalRedirect(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction("LoginWith2FA", new { ReturnUrl = returnUrl, model.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToAction("Lockout");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "Admin")] 
        [HttpGet]
        public async Task<IActionResult> AllEmployee()
        {
            var users = _userManager.Users.ToList();
            _logger.LogInformation("Fetched all users.");
            return View(users);
        }


        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound("User ID not provided.");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found.");

            var profile = await _context.profiles.FirstOrDefaultAsync(p => p.EmployeeId == Guid.Parse(user.Id));

            var model = new EmployeeViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = profile?.Name ?? "",
                Department = profile?.Department
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound("User not found.");

            var profile = await _context.profiles.FirstOrDefaultAsync(p => p.EmployeeId == Guid.Parse(user.Id));

            user.Email = model.Email;
            await _userManager.UpdateAsync(user);

            if (profile != null)
            {
                profile.Name = model.Name;
                profile.Department = model.Department;
                _context.profiles.Update(profile);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound("User ID not provided.");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found.");

            return View(user);
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found.");

            var profile = await _context.profiles.FirstOrDefaultAsync(p => p.EmployeeId == Guid.Parse(user.Id));

            if (profile != null) _context.profiles.Remove(profile);
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
