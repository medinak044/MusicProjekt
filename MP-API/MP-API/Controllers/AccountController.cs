using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MP_API.Data;
using MP_API.Data.DTOs;
using MP_API.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MP_API.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("fixed")]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _appSettings;
    private readonly TokenValidationParameters _tokenValidationParams;
    private readonly IMapper _mapper;

    public AccountController(
    //    DataContext context,
    //IUnitOfWork unitOfWork,
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration appSettings, // (Access app settings data for Jwt token)
    TokenValidationParameters tokenValidationParams, // (Jwt token)
    IMapper mapper
        )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _appSettings = appSettings;
        _tokenValidationParams = tokenValidationParams;
        _mapper = mapper;
    }

    [HttpGet($"{nameof(this.GetAllUsers)}")]
    public async Task<ActionResult> GetAllUsers()
    {
        return Ok();
    }

    //[HttpGet($"{nameof(this.GetSomeUsers)}")]
    //public async Task<ActionResult> GetSomeUsers()
    //{
    //    return Ok();
    //}

    // If getting the current user's data, have the client send the id from the browser cookie
    [HttpGet($"{nameof(this.GetUserById)}")]
    public async Task<ActionResult> GetUserById()
    {
        return Ok();
    }

    [HttpGet($"{nameof(this.AddUser)}")]
    public async Task<ActionResult> AddUser()
    {
        return Ok();
    }

    [HttpPost($"{nameof(this.Register)}")]
    public async Task<ActionResult> Register(AppUserRegistrationDto requestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        // Check if email already exists
        AppUser existingUser = await _userManager.FindByEmailAsync(requestDto.Email);
        if (existingUser != null)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "This email address is already in use" }
            });
        }
        AppUser newUser = _mapper.Map<AppUser>(requestDto);

        var newUserIsCreated = await _userManager.CreateAsync(newUser, requestDto.Password);
        if (!newUserIsCreated.Succeeded)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "Server error" }
            });
        }

        // Prevent new user from being created if "AppUser" default role don't exist
        if (await _roleManager.FindByNameAsync("AppUser") == null)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "AppUser default account role doesn't exist" }
            });
        }

        // Grab the new user from db
        AppUser newUserFromDb = await _userManager.FindByEmailAsync(requestDto.Email);
        if (newUserFromDb == null)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "Failed to return newly created user from database" }
            });
        }

        //// Add user to a default role after user has been created
        //await _userManager.AddToRoleAsync(newUser, RoleNames.RoleTypeEnum.AppUser.ToString());

        //// Give token to user (to be stored in browser local storage client-side)
        //ApiResponse jwtTokenResult = await GenerateJwtTokenAsync(newUserFromDb);

        // Prepare to send data back to the client to indicate user has logged in
        AppUserLoggedInDto loggedInUser = _mapper.Map<AppUserLoggedInDto>(newUserFromDb);

        //// Then map the token data
        //_mapper.Map<ApiResponse, AppUserLoggedInDto>(jwtTokenResult, loggedInUser);

        return Ok(loggedInUser);
    }

    [HttpPost($"{nameof(this.Login)}")]
    public async Task<ActionResult> Login(AppUserLoginDto loginRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        // Check if user exists
        AppUser existingUser = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (existingUser == null)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "Email doesn't exist" }
            });
        }

        // Verify password
        var passwordIsCorrect = await _userManager.CheckPasswordAsync(existingUser, loginRequest.Password);
        if (!passwordIsCorrect)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "Invalid credentials" }
            });
        }

        //// Give token to user (to be stored in browser local storage client-side)
        //ApiResponse jwtTokenResult = await GenerateJwtTokenAsync(existingUser);

        // Map user details
        AppUserLoggedInDto loggedInUser = _mapper.Map<AppUserLoggedInDto>(existingUser);
        //// Then map the token data
        //_mapper.Map<ApiResponse, AppUserLoggedInDto>(jwtTokenResult, loggedInUser);

        return Ok(loggedInUser);
    }

    [HttpPost($"{nameof(this.UpdateUser)}")]
    public async Task<ActionResult> UpdateUser(AppUserDto updatedUserDto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        // Reference user
        var existingUser = await _userManager.FindByIdAsync(updatedUserDto.Id);
        if (existingUser == null)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "Email doesn't exist" }
            });
        }

        #region DEMO: Prevent demo Admin user from changing/deleting users
        if (existingUser.Email == "admin@example.com")
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "Demo Admin not allowed to edit user data" }
            });
        }
        #endregion

        // Map values
        existingUser = _mapper.Map<AppUserDto, AppUser>(updatedUserDto, existingUser);

        // Save updated values to db
        await _userManager.UpdateAsync(existingUser);

        //return Ok("User successfully updated");
        return Ok(existingUser);
    }

    [HttpDelete($"{nameof(this.DeleteUser)}")]
    public async Task<ActionResult> DeleteUser(string userId)
    {
        // Reference user by id
        var existingUser = await _userManager.FindByIdAsync(userId);
        if (existingUser == null)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "User doesn't exist" }
            });
        }

        #region DEMO: Prevent demo Admin user from changing/deleting users
        if (existingUser.Email == "admin@example.com")
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                Messages = new List<string>() { "Demo Admin not allowed to edit user data" }
            });
        }
        #endregion

        // Delete user from db
        await _userManager.DeleteAsync(existingUser);

        return Ok("User successfully deleted");
    }

    //[HttpGet($"{nameof(this.Default)}")]
    //public async Task<ActionResult> Default()
    //{
    //    // Validations
    //    var result = new RequestResult()
    //    {
    //        Success = true,
    //        Messages = new List<string>()
    //    };

    //    #region Account Roles (Identity framework)
    //    var accountRoleNames = new List<string>();

    //    if (await _roleManager.FindByNameAsync("AppUser") == null)
    //    {
    //        accountRoleNames.Add("AppUser");
    //    }
    //    if (await _roleManager.FindByNameAsync("Admin") == null)
    //    {
    //        accountRoleNames.Add("Admin");
    //    }

    //    foreach (var accountRoleName in accountRoleNames)
    //    {
    //        await _roleManager.CreateAsync(new IdentityRole(accountRoleName));
    //        result.Messages.Add($"{accountRoleName} IdentityRole added");
    //    }
    //    #endregion

    //    #region AppUsers (Identity framework)
    //    var demoIdentityPassword = "Password!23";

    //    if (await _userManager.FindByEmailAsync("admin@example.com") == null)
    //    {
    //        var adminUser = new AppUser()
    //        {
    //            FirstName = "Admin_F",
    //            LastName = "Admin_L",
    //            UserName = "Admin_UserName",
    //            Email = "admin@example.com",
    //        };
    //        await _userManager.CreateAsync(adminUser, demoIdentityPassword);
    //        // After user is created, add role
    //        var foundUser = await _userManager.FindByEmailAsync(adminUser.Email);
    //        await _userManager.AddToRoleAsync(foundUser, "AppUser");
    //        await _userManager.AddToRoleAsync(foundUser, "Admin");
    //        result.Messages.Add("Admin user added");
    //    }

    //    if (await _userManager.FindByEmailAsync("appuser@example.com") == null)
    //    {
    //        var appUser = new AppUser()
    //        {
    //            FirstName = "AppUser_F",
    //            LastName = "AppUser_L",
    //            UserName = "AppUser_UserName",
    //            Email = "appuser@example.com",
    //        };
    //        await _userManager.CreateAsync(appUser, demoIdentityPassword);
    //        // After user is created, add role
    //        var foundUser = await _userManager.FindByEmailAsync(appUser.Email);
    //        await _userManager.AddToRoleAsync(foundUser, "AppUser");
    //        result.Messages.Add("App user added");
    //    }
    //    #endregion

    //    #region Event roles
    //    var eventRoles = new List<EventRole>();

    //    if (_unitOfWork.EventRoles.GetSome(e => e.Role == "Attendee").Count() == 0)
    //    {
    //        var eventRole = new EventRole() { Role = "Attendee" };
    //        eventRoles.Add(eventRole);
    //    }
    //    if (_unitOfWork.EventRoles.GetSome(e => e.Role == "Organizer").Count() == 0)
    //    {
    //        var eventRole = new EventRole() { Role = "Organizer" };
    //        eventRoles.Add(eventRole);
    //    }
    //    if (_unitOfWork.EventRoles.GetSome(e => e.Role == "Owner").Count() == 0)
    //    {
    //        var eventRole = new EventRole() { Role = "Owner" };
    //        eventRoles.Add(eventRole);
    //    }

    //    foreach (var eventRole in eventRoles)
    //    {
    //        await _unitOfWork.EventRoles.AddAsync(eventRole);
    //        if (await _unitOfWork.SaveAsync() == false)
    //        {
    //            return BadRequest(new RequestResult()
    //            {
    //                Success = false,
    //                Messages = new List<string>() { "Something went wrong while saving" }
    //            });
    //        }
    //        else
    //        {
    //            result.Messages.Add($"{eventRole.Role} EventRole added");
    //        }
    //    }

    //    #endregion


    //    return Ok(result);
    //}

    #region Jwt Token logic
    //private async Task<AuthResult> GenerateJwtTokenAsync(AppUser user)
    //{

    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.GetSection("JwtConfig:Secret").Value));
    //    var currentDate = DateTime.UtcNow;
    //    var claims = await GetAllValidClaimsAsync(user);

    //    // Token descriptor
    //    var tokenDescriptor = new SecurityTokenDescriptor()
    //    {
    //        Subject = new ClaimsIdentity(claims),
    //        //Expires = currentDate.AddSeconds(10), // Temp: For refresh token demo purposes
    //        Expires = currentDate.AddDays(1),
    //        NotBefore = currentDate,
    //        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
    //    };

    //    var jwtTokenHandler = new JwtSecurityTokenHandler();

    //    var token = jwtTokenHandler.CreateToken(tokenDescriptor);
    //    var jwtToken = jwtTokenHandler.WriteToken(token);

    //    var refreshToken = new RefreshToken()
    //    {
    //        JwtId = token.Id,
    //        IsUsed = false,
    //        IsRevoked = false,
    //        UserId = user.Id,
    //        AddedDate = currentDate,
    //        ExpiryDate = currentDate.AddMonths(6),
    //        Token = RandomString(35) + Guid.NewGuid(),
    //    };

    //    await _context.RefreshTokens.AddAsync(refreshToken); // Add changes to memory
    //    await _context.SaveChangesAsync(); // Save changes to db

    //    return new AuthResult()
    //    {
    //        Token = jwtToken,
    //        RefreshToken = refreshToken.Token,
    //        Success = true,
    //    };
    //}

    //private async Task<List<Claim>> GetAllValidClaimsAsync(AppUser user)
    //{
    //    var claims = new List<Claim>
    //    {
    //        new Claim("Id", user.Id), // There is no ClaimTypes.Id
    //        new Claim(ClaimTypes.NameIdentifier, user.UserName),
    //        new Claim(ClaimTypes.Email, user.Email),
    //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // For "JwtId"
    //        // new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
    //        // (The role claim will be added here)
    //    };

    //    // Getting the claims that we have assigned to the user
    //    var userClaims = await _userManager.GetClaimsAsync(user);
    //    claims.AddRange(userClaims);

    //    // Get the user role, convert it, and add it to the claims
    //    var userRoles = await _userManager.GetRolesAsync(user);
    //    foreach (var userRole in userRoles)
    //    {
    //        var role = await _roleManager.FindByNameAsync(userRole);

    //        if (role != null)
    //        {
    //            //claims.Add(new Claim("Roles", userRole));
    //            claims.Add(new Claim(ClaimTypes.Role, userRole));

    //            var roleClaims = await _roleManager.GetClaimsAsync(role);
    //            foreach (var roleClaim in roleClaims)
    //            { claims.Add(roleClaim); }
    //        }
    //    }

    //    return claims;
    //}

    //private async Task<AuthResult> VerifyAndGenerateToken(TokenRequestDto tokenRequest)
    //{
    //    var jwtTokenHandler = new JwtSecurityTokenHandler();

    //    try // Run the token request through validations
    //    {
    //        // Check that the string is actually in jwt token format
    //        // (The token validation parameters were defined in the Program.cs class)
    //        var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token,
    //            _tokenValidationParams, out var validatedToken);

    //        // Check if the encryption algorithm matches
    //        if (validatedToken is JwtSecurityToken jwtSecurityToken)
    //        {
    //            bool result = jwtSecurityToken.Header.Alg
    //                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCulture);

    //            if (result == false)
    //                return null;
    //        }

    //        // Check if token has expired (don't generate new token if current token is still usable)
    //        // "long" was used because of the long utc time string
    //        var utcExpiryDate = long.Parse(tokenInVerification.Claims
    //            .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

    //        // Convert into a usable date type
    //        var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

    //        if (expiryDate > DateTime.UtcNow)
    //        {
    //            return new AuthResult()
    //            {
    //                Success = false,
    //                Messages = new List<string>() { "Token has not yet expired" }
    //            };
    //        }

    //        // Check if token exists in db
    //        var storedToken = await _context.RefreshTokens
    //            .FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

    //        if (storedToken == null)
    //        {
    //            return new AuthResult()
    //            {
    //                Success = false,
    //                Messages = new List<string>() { "Token does not exist" }
    //            };
    //        }

    //        // Check if token is already used
    //        if (storedToken.IsUsed)
    //        {
    //            return new AuthResult()
    //            {
    //                Success = false,
    //                Messages = new List<string>() { "Token has been used" }
    //            };
    //        }

    //        // Check if token has been revoked
    //        if (storedToken.IsRevoked)
    //        {
    //            return new AuthResult()
    //            {
    //                Success = false,
    //                Messages = new List<string>() { "Token has been revoked" }
    //            };
    //        }

    //        // Check if jti matches the id of the refresh token that exists in our db (validate the id)
    //        var jti = tokenInVerification.Claims
    //            .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

    //        if (storedToken.JwtId != jti)
    //        {
    //            return new AuthResult()
    //            {
    //                Success = false,
    //                Messages = new List<string>() { "Token does not match" }
    //            };
    //        }

    //        // First, update current token
    //        storedToken.IsUsed = true; // Prevent the current token from being used in the future
    //        _context.RefreshTokens.Update(storedToken);
    //        await _context.SaveChangesAsync(); // Save changes

    //        // Then, generate a new jwt token, then assign it to the user
    //        var dbUser = await _userManager.FindByIdAsync(storedToken.UserId); // Find the AppUser by the user id on the current token
    //        return await GenerateJwtTokenAsync(dbUser);
    //    }
    //    catch (Exception ex)
    //    {
    //        return null;
    //    }
    //}

    //private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    //{
    //    var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    //    dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();
    //    return dateTimeVal;
    //}

    //private string RandomString(int length)
    //{
    //    var random = new Random();
    //    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    //    return new string(Enumerable.Repeat(chars, length)
    //        .Select(x => x[random.Next(x.Length)]).ToArray());
    //}

    #endregion
}


