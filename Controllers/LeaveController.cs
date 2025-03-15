using EmployeeLeave.Data;
using EmployeeLeave.Data.Table;
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

        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> MyLeaves(Guid employeeId)
        {
            var leaves = await _context.leaves
                .Where(l => l.EmployeeId == employeeId)
                .ToListAsync();

            return View(leaves);
        }


        [HttpPost]
        public async Task<IActionResult> ApplyLeave(string Reason, Guid EmployeeId)
        {
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

            return View();
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
