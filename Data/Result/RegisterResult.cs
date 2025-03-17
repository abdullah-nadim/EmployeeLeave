namespace EmployeeLeave.Data.Result
{
    public class RegisterResult
    {
        public bool Succeeded { get; set; }
        public List<string>? Errors { get; set; }
    }
}
