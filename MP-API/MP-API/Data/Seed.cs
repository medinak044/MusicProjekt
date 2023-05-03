using Microsoft.AspNetCore.Identity;
using MP_API.Data.Models;

namespace MP_API.Data;

public class Seed
{
    private readonly DataContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public Seed(
        DataContext context,
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Make sure default user data, account roles, exists for the app to function
    public async void SeedDataContext()
    {
        #region Account Roles (Identity framework)
        if (await _roleManager.FindByNameAsync("AppUser") == null)
        {
            await _roleManager.CreateAsync(new IdentityRole("AppUser"));
        }
        if (await _roleManager.FindByNameAsync("Admin") == null)
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        #endregion

        #region AppUsers (Identity framework)
        var demoIdentityPassword = "Password!23";

        if (await _userManager.FindByEmailAsync("admin@example.com") == null)
        {
            var adminUser = new AppUser()
            {
                FirstName = "Admin_F",
                LastName = "Admin_L",
                UserName = "Admin_UserName",
                Email = "admin@example.com",
            };
            await _userManager.CreateAsync(adminUser, demoIdentityPassword);
            // After user is created, add role
            var foundUser = await _userManager.FindByEmailAsync(adminUser.Email);
            await _userManager.AddToRoleAsync(foundUser, "AppUser");
            await _userManager.AddToRoleAsync(foundUser, "Admin");
        }

        if (await _userManager.FindByEmailAsync("appuser@example.com") == null)
        {
            var appUser = new AppUser()
            {
                FirstName = "AppUser_F",
                LastName = "AppUser_L",
                UserName = "AppUser_UserName",
                Email = "appuser@example.com",
            };
            await _userManager.CreateAsync(appUser, demoIdentityPassword);
            // After user is created, add role
            var foundUser = await _userManager.FindByEmailAsync(appUser.Email);
            await _userManager.AddToRoleAsync(foundUser, "AppUser");
        }
        #endregion

        #region Project roles
        //var eventRoles = new List<EventRole>();

        //if (_unitOfWork.EventRoles.GetSome(e => e.Role == "Attendee") == null)
        //{
        //    var eventRole = new EventRole()
        //    {
        //        Role = "Owner"
        //    };
        //    eventRoles.Add(eventRole);
        //}
        //if (_unitOfWork.EventRoles.GetSome(e => e.Role == "Organizer") == null)
        //{
        //    var eventRole = new EventRole()
        //    {
        //        Role = "Owner"
        //    };
        //    eventRoles.Add(eventRole);
        //}
        //if (_unitOfWork.EventRoles.GetSome(e => e.Role == "Owner") == null)
        //{
        //    var eventRole = new EventRole()
        //    {
        //        Role = "Owner"
        //    };
        //    eventRoles.Add(eventRole);
        //}

        //await _context.EventRoles.AddRangeAsync(eventRoles);
        #endregion
    }
}
