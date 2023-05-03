using System.ComponentModel.DataAnnotations;

namespace MP_API.Data.DTOs;

public class AppUserLoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
