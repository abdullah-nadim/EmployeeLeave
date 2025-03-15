using Microsoft.AspNetCore.Identity;

namespace EmployeeLeave.Data.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string?  Name { get; set; }

    }
}
