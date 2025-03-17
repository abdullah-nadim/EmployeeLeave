namespace EmployeeLeave.Models
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalLeaves { get; set; }
        public int AcceptedLeaves { get; set; }
        public int RejectedLeaves { get; set; }
        public int PendingLeaves { get; set; }
    }
}
