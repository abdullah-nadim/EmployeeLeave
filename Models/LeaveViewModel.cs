namespace EmployeeLeave.Models
{
    public class LeaveViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; } = "Pending";
        public string Reason { get; set; } = string.Empty;
        public Guid EmployeeId { get; set; }

        
        public string EmployeeName { get; set; }
        public string Department { get; set; }
    }
}
