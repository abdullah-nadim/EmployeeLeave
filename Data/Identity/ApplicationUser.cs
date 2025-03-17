using Microsoft.AspNetCore.Identity;

namespace EmployeeLeave.Data.Identity
{
    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }
    public class ApplicationUser : IdentityUser
    {
        public string?  Name { get; set; }
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    }
}
