using EmployeeLeave.Data;
using EmployeeLeave.Data.Identity;
using EmployeeLeave.Data.Table;
using EmployeeLeave.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Authorize] // Ensure only logged-in users can access the profile
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User); // Get logged-in user
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var employeeId = Guid.Parse(user.Id); // Convert user Id to Guid
        var profile = await _context.profiles.FirstOrDefaultAsync(p => p.EmployeeId == employeeId); // Search by EmployeeId

        if (profile == null)
        {
            return NotFound("Profile not found. Please contact admin.");
        }

        var model = new ProfileViewModel
        {
            Name = profile.Name,
            EmployeeId = profile.EmployeeId,
            Department = profile.Department ?? "Not Assigned",
            Email = user.Email // Get Email from Identity
        };

        return View(model);
    }
}
