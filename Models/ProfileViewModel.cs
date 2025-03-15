using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeave.Models
{
    public class ProfileViewModel 
    {
        public string Name { get; set; }
        public Guid EmployeeId { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
    }
}
