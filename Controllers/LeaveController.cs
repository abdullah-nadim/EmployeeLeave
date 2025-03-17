using EmployeeLeave.Data;
using EmployeeLeave.Data.Identity;
using EmployeeLeave.Data.Table;
using EmployeeLeave.Models;
using EmployeeLeave.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeLeave.Controllers
{
    public class LeaveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILeaveService _leaveService;

        public LeaveController(ApplicationDbContext context, ILeaveService leaveService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _leaveService = leaveService;
        }

       
        public async Task<IActionResult> MyLeaves()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var employeeId = Guid.Parse(user.Id);
            var leaves = await _leaveService.GetMyLeavesAsync(employeeId);
            return View(leaves);
        }

        public async Task<IActionResult> LeaveList()
        {
            var leaveList = await _leaveService.GetAllLeavesAsync();
            return View(leaveList);
        }

        public async Task<IActionResult> ApplyLeave(string Reason)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var employeeId = Guid.Parse(user.Id);
            var success = await _leaveService.ApplyLeaveAsync(employeeId, Reason);

            return success ? RedirectToAction("MyLeaves") : RedirectToAction("Index", "Profile");
        }



        [HttpPost]
        public async Task<IActionResult> ApproveLeave(int leaveId)
        {
            var success = await _leaveService.UpdateLeaveStatusAsync(leaveId, "Approved");
            return success ? RedirectToAction("LeaveList") : NotFound();
        }

       
        [HttpPost]
        public async Task<IActionResult> RejectLeave(int leaveId)
        {
            var success = await _leaveService.UpdateLeaveStatusAsync(leaveId, "Rejected");
            return success ? RedirectToAction("LeaveList") : NotFound();
        }
    }
}
