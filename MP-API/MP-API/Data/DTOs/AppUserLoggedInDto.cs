namespace MP_API.Data.DTOs;

public class AppUserLoggedInDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; } // Includes Id, UserName, Email in claims
    public string RefreshToken { get; set; }
}
