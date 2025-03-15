using System.ComponentModel.DataAnnotations;

namespace EmployeeLeave.Models
{
    public class EmployeeViewModel
    {
        public string Id { get; set; } // Identity User ID (Guid as String)

        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; } // Comes from Profile

        public string? Department { get; set; } // Comes from Profile (Editable by Admin)
    }
}
