using EmployeeLeave.Data;
using EmployeeLeave.Data.Identity;
using EmployeeLeave.Data.Table;
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

        public LeaveController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       
        public async Task<IActionResult> MyLeaves()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var employeeId = Guid.Parse(user.Id);
            var leaves = await _context.leaves
                .Where(l => l.EmployeeId == employeeId)
                .ToListAsync();

            return View(leaves);
        }


        public async Task<IActionResult> ApplyLeave(string Reason)
        {
            var user = await _userManager.GetUserAsync(User); 
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var EmployeeId = Guid.Parse(user.Id);

            if (!string.IsNullOrEmpty(Reason) && EmployeeId != Guid.Empty)
            {
                var leave = new Leave
                {
                    Reason = Reason,
                    EmployeeId = EmployeeId,
                    Status = "Pending" 
                };

                _context.leaves.Add(leave);
                await _context.SaveChangesAsync();

               
                return RedirectToAction("MyLeaves");
            }

            return RedirectToAction("Index","Profile");
        }



        [HttpPost]
        public async Task<IActionResult> ApproveLeave(int leaveId)
        {
            var leave = await _context.leaves.FindAsync(leaveId);
            if (leave == null)
            {
                return NotFound();
            }

            leave.Status = "Approved"; // Update status to Approved
            _context.leaves.Update(leave);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyLeaves), new { employeeId = leave.EmployeeId });
        }

       
        [HttpPost]
        public async Task<IActionResult> RejectLeave(int leaveId)
        {
            var leave = await _context.leaves.FindAsync(leaveId);
            if (leave == null)
            {
                return NotFound();
            }

            leave.Status = "Rejected"; 
            _context.leaves.Update(leave);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyLeaves), new { employeeId = leave.EmployeeId });
        }
    }
}
