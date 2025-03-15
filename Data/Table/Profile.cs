using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeLeave.Data.Table
{
    [Table("Profile")]
    public class Profile
    { 
        public int Id { get; set; }

        [Required]
        public string? Name{ get; set; }

        public string? Department { get; set; }

        public Guid EmployeeId { get; set; }

    }
}
