public class LoginViewModel
{
    public bool UseEmail { get; set; } = true;
    public string Email { get; set; }
    public string EmployeeId { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
    public string ErrorMessage { get; set; }
}