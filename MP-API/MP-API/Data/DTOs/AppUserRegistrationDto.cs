using System.ComponentModel.DataAnnotations;

namespace MP_API.Data.DTOs;

public class AppUserRegistrationDto
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required] [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
