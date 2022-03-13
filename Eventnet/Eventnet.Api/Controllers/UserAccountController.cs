using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Eventnet.DataAccess;
using Eventnet.Domain;
using Eventnet.Models;
using Eventnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Eventnet.Controllers;

[Route("api/auth")]
public class UserAccountController : Controller
{
    private readonly UserManager<UserEntity> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IJwtAuthService jwtAuthService;
    private readonly IEmailService emailService;

    public UserAccountController(UserManager<UserEntity> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtAuthService jwtAuthService,
        IEmailService emailService)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.jwtAuthService = jwtAuthService;
        this.emailService = emailService;
    }

    [HttpPost("login")]
    [Produces(typeof(LoginResult))]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var user = await userManager.FindByNameAsync(loginModel.UserName)
            ?? await userManager.FindByEmailAsync(loginModel.Email);

        if (user == null || !await userManager.CheckPasswordAsync(user, loginModel.Password))
            return Unauthorized();

        if (!user.EmailConfirmed)
            return BadRequest(new { Status = "please confirm your email" });

        var roles = await userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        authClaims.AddRange(roles
            .Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var (jwtSecurityToken, refreshToken) = jwtAuthService.GenerateTokens(user.UserName, authClaims, DateTime.Now);

        return Ok(new LoginResult(
            new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            jwtSecurityToken.ValidTo,
            refreshToken.TokenString,
            user,
            roles
        ));
    }

    [HttpPost("logout")]
    [Authorize]
    public ActionResult Logout()
    {
        var userName = User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        if (userName == null)
            return Unauthorized();

        jwtAuthService.RemoveRefreshTokenByUserName(userName);
        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var userExists = await userManager.FindByNameAsync(registerModel.UserName);

        if (userExists != null)
            return BadRequest("User already exists");

        var user = new UserEntity
        {
            Email = registerModel.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerModel.UserName,
            PhoneNumber = registerModel.Phone
        };

        var result = await userManager.CreateAsync(user, registerModel.Password);

        if (!await roleManager.RoleExistsAsync(UserRoles.User))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
        await userManager.AddToRoleAsync(user, UserRoles.User);

        if (!result.Succeeded)
            return BadRequest("User creation failed");

        await SendEmailConfirmationMessageAsync(user);

        // TODO: replace with CreatedAtAction when implement UserController 
        return Ok(new RegisterResult("User created successfully. Please check your email", user));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] RestorePasswordModel restorePasswordModel)
    {
        if (restorePasswordModel.OldPassword == restorePasswordModel.NewPassword)
            return BadRequest("Passwords should be different");

        var userName = User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        if (userName == null)
            return NotFound();

        var user = await userManager.FindByNameAsync(userName);

        var changePasswordResult = await userManager.ChangePasswordAsync(user,
            restorePasswordModel.OldPassword, restorePasswordModel.NewPassword);

        if (changePasswordResult.Succeeded)
            return Ok();

        var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
        return BadRequest(errors);
    }

    [HttpGet("email-confirmation-message")]
    public async Task<IActionResult> SendEmailConfirmation(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user == null)
            return NotFound();

        await SendEmailConfirmationMessageAsync(user);

        return Ok();
    }

    [HttpGet("confirm-email", Name = nameof(ConfirmEmail))]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest();

        var result = await userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
            return Ok("Email confirmed");

        return BadRequest();
    }

    public async Task SendEmailConfirmationMessageAsync(UserEntity user)
    {
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Link(nameof(ConfirmEmail), new { userId = user.Id, code });

        await emailService.SendEmailAsync(
            user.Email,
            "jopa",
            $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");
    }
}