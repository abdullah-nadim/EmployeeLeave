using EmployeeLeave.Data;
using EmployeeLeave.Data.Identity;
using EmployeeLeave.Data.Result;
using EmployeeLeave.Data.Table;
using EmployeeLeave.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployeeLeave.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        public async Task<RegisterResult> RegisterAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return new RegisterResult { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToList() };

            await _userManager.AddToRoleAsync(user, "Employee");

            var profile = new Profile
            {
                Name = model.Name,
                EmployeeId = Guid.Parse(user.Id),
                Department = null
            };

            _context.profiles.Add(profile);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered and profile created.");
            await _signInManager.SignInAsync(user, isPersistent: false);

            return new RegisterResult { Succeeded = true };
        }

        public async Task<LoginResult> LoginAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            return new LoginResult
            {
                Succeeded = result.Succeeded,
                RequiresTwoFactor = result.RequiresTwoFactor,
                IsLockedOut = result.IsLockedOut
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<List<ApplicationUser>> GetAllEmployeesAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<EmployeeViewModel> GetEmployeeByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var profile = await _context.profiles.FirstOrDefaultAsync(p => p.EmployeeId == Guid.Parse(user.Id));

            return new EmployeeViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = profile?.Name ?? "",
                Department = profile?.Department
            };
        }

        //public async Task<EmployeeViewModel> GetByIdAsync(string id)
        //{
        //    if (string.IsNullOrEmpty(id)) return NotFound("User ID not provided.");

        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null) return NotFound("User not found.");

        //    return new ApplicationUser
        //    {
        //        Id = user.Id,
        //        Email = user.Email
        //    };

        //}

        public async Task<DashboardViewModel> Dashboard()
        {
            var totalEmployees = await _userManager.Users.CountAsync();
            var totalLeaves = await _context.leaves.CountAsync();
            var acceptedLeaves = await _context.leaves.CountAsync(l => l.Status == "Approved");
            var rejectedLeaves = await _context.leaves.CountAsync(l => l.Status == "Rejected");
            var pendingLeaves = await _context.leaves.CountAsync(l => l.Status == "Pending");

            var model = new DashboardViewModel
            {
                TotalEmployees = totalEmployees,
                TotalLeaves = totalLeaves,
                AcceptedLeaves = acceptedLeaves,
                RejectedLeaves = rejectedLeaves,
                PendingLeaves = pendingLeaves
            };
            return model;
        }


        public async Task<bool> UpdateEmployeeAsync(EmployeeViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return false;

            user.Email = model.Email;
            await _userManager.UpdateAsync(user);

            var profile = await _context.profiles.FirstOrDefaultAsync(p => p.EmployeeId == Guid.Parse(user.Id));
            if (profile != null)
            {
                profile.Name = model.Name;
                profile.Department = model.Department;
                _context.profiles.Update(profile);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var profile = await _context.profiles.FirstOrDefaultAsync(p => p.EmployeeId == Guid.Parse(user.Id));
            if (profile != null) _context.profiles.Remove(profile);

            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
