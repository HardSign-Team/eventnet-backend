using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Models;
using Eventnet.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Eventnet.Controllers;

[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IJwtAuthService jwtAuthService;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly UserManager<UserEntity> userManager;

    public AuthController(UserManager<UserEntity> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtAuthService jwtAuthService)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.jwtAuthService = jwtAuthService;
    }

    [HttpPost("login")]
    [Produces(typeof(LoginResponseModel))]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var user = await userManager.FindByNameAsync(loginModel.Username);

        if (user == null || !await userManager.CheckPasswordAsync(user, loginModel.Password))
            return BadRequest(new { Error = "Incorrect login or password" });

        var userRoles = await userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        authClaims.AddRange(userRoles
            .Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var token = jwtAuthService.GenerateTokens(authClaims.ToArray(), DateTime.Now);

        return Ok(new LoginResponseModel(
            new JwtSecurityTokenHandler().WriteToken(token.AccessToken),
            token.AccessToken.ValidTo,
            user,
            userRoles
        ));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var userExists = await userManager.FindByNameAsync(registerModel.Username);

        if (userExists != null)
            return BadRequest("User already exists");

        var user = new UserEntity
        {
            Email = registerModel.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerModel.Username,
            PhoneNumber = registerModel.Phone
        };

        var result = await userManager.CreateAsync(user, registerModel.Password);

        if (!await roleManager.RoleExistsAsync(UserRoles.User))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        await userManager.AddToRoleAsync(user, UserRoles.User);

        if (!result.Succeeded)
            return BadRequest("User creation failed");

        // TODO: replace with CreatedAtAction when implement UserController 
        return Ok(new
        {
            Message = "User created successfully",
            User = user
        });
    }
}