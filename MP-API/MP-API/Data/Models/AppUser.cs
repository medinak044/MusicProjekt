using Microsoft.AspNetCore.Identity;

namespace MP_API.Data.Models;

public class AppUser: IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    // Collection of Workspaces
    //public ICollection<Organization> Organizations { get; set; }
}
