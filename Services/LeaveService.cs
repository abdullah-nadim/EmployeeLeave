using EmployeeLeave.Data;
using EmployeeLeave.Data.Table;
using EmployeeLeave.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeLeave.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ApplicationDbContext _context;

        public LeaveService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Leave>> GetMyLeavesAsync(Guid employeeId)
        {
            return await _context.leaves
                .Where(l => l.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<List<LeaveViewModel>> GetAllLeavesAsync()
        {
            return await _context.leaves
                .Join(_context.profiles,
                    leave => leave.EmployeeId,
                    profile => profile.EmployeeId,
                    (leave, profile) => new LeaveViewModel
                    {
                        Id = leave.Id,
                        Status = leave.Status,
                        Reason = leave.Reason,
                        EmployeeId = leave.EmployeeId,
                        EmployeeName = profile.Name,
                        Department = profile.Department
                    })
                .ToListAsync();
        }

        public async Task<bool> ApplyLeaveAsync(Guid employeeId, string reason)
        {
            if (string.IsNullOrEmpty(reason) || employeeId == Guid.Empty)
                return false;

            var leave = new Leave
            {
                Reason = reason,
                EmployeeId = employeeId,
                Status = "Pending"
            };

            _context.leaves.Add(leave);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateLeaveStatusAsync(int leaveId, string status)
        {
            var leave = await _context.leaves.FindAsync(leaveId);
            if (leave == null)
                return false;

            leave.Status = status;
            _context.leaves.Update(leave);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
