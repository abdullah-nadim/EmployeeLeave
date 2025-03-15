using Microsoft.AspNetCore.WebUtilities;

namespace EmployeeLeave.Data.Table
{

    public class Leave
    {
        public int Id { get; set; }

        public string Status { get; set; } = "Pending";

        public string Reason { get; set; } = string.Empty;

        public Guid EmployeeId { get; set; }
    }
}
