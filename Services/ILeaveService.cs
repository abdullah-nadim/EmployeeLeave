using EmployeeLeave.Data.Table;
using EmployeeLeave.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeLeave.Services
{
    public interface ILeaveService
    {
        Task<List<Leave>> GetMyLeavesAsync(Guid employeeId);
        Task<List<LeaveViewModel>> GetAllLeavesAsync();
        Task<bool> ApplyLeaveAsync(Guid employeeId, string reason);
        Task<bool> UpdateLeaveStatusAsync(int leaveId, string status);
    }
}
