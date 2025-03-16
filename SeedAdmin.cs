using EmployeeLeave.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace EmployeeLeave
{
    public class SeedAdmin
    {
        public static async Task SeedRolesAndAdmin(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminRole = "Admin";
            string employeeRole = "Employee";

            // Check if roles exist, otherwise create them
            if (!await roleManager.RoleExistsAsync(adminRole))
                await roleManager.CreateAsync(new IdentityRole(adminRole));

            if (!await roleManager.RoleExistsAsync(employeeRole))
                await roleManager.CreateAsync(new IdentityRole(employeeRole));

            // Check if admin user exists
            string adminEmail = "admin@employeeleave.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123"); // Default password

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }
        }

    }
}
