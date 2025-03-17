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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using EmployeeLeave.Services;

namespace EmployeeLeave.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
             IAccountService accountService)
        {
            _userManager = userManager;
            _accountService = accountService;
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _accountService.Dashboard();

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
                //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.RegisterAsync(model);

            if (result.Succeeded) return RedirectToAction("Index", "Home");

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _accountService.LoginAsync(model);

            if (result.Succeeded) return RedirectToAction("Index", "Home");

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid login attempt.");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login");
        }

        [Authorize(Roles = "Admin")] 
        [HttpGet]
        public async Task<IActionResult> AllEmployee()
        {
            var employees = await _accountService.GetAllEmployeesAsync();
            return View(employees);
        }


        public async Task<IActionResult> Edit(string id)
        {
            var employee = await _accountService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _accountService.UpdateEmployeeAsync(model);
            if (!success) return NotFound();

            return RedirectToAction(nameof(AllEmployee));
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
            var success = await _accountService.DeleteEmployeeAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(AllEmployee));
        }

        [HttpGet]
        public IActionResult PendingApproval()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ApproveUsers()
        {
            var pendingUsers = await _userManager.Users
                .Where(u => u.Status == ApprovalStatus.Pending)
                .ToListAsync();

            return View(pendingUsers);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangeApprovalStatus(string userId, ApprovalStatus status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.Status = status;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("ApproveUsers");
        }
    }
}
