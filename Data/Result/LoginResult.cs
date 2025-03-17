namespace EmployeeLeave.Data.Result
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool IsLockedOut { get; set; }

        public String? ErrorMessage { get; set; }
    }
}
