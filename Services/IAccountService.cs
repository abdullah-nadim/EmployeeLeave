using EmployeeLeave.Models;
using EmployeeLeave.Data.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using EmployeeLeave.Data.Result;

namespace EmployeeLeave.Services
{
    public interface IAccountService
    {
        Task<DashboardViewModel> Dashboard();
        Task<RegisterResult> RegisterAsync(RegisterViewModel model);
        Task<LoginResult> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<List<ApplicationUser>> GetAllEmployeesAsync();
        Task<EmployeeViewModel> GetEmployeeByIdAsync(string id);
        Task<bool> UpdateEmployeeAsync(EmployeeViewModel model);
        Task<bool> DeleteEmployeeAsync(string id);
    }
}
