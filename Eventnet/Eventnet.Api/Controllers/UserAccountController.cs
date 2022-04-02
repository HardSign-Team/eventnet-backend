using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Models;
using Eventnet.Domain;
using Eventnet.Models;
using Eventnet.Models.Authentication;
using Eventnet.Models.Authentication.Tokens;
using Eventnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Eventnet.Controllers;

[Route("api/auth")]
public class UserAccountController : Controller
{
    private readonly UserManager<UserEntity> userManager;
    private readonly CurrentUserService currentUserService;
    private readonly IJwtAuthService jwtAuthService;
    private readonly IEmailService emailService;
    private readonly IMapper mapper;

    public UserAccountController(UserManager<UserEntity> userManager,
        CurrentUserService currentUserService,
        IJwtAuthService jwtAuthService,
        IEmailService emailService,
        IMapper mapper)
    {
        this.userManager = userManager;
        this.currentUserService = currentUserService;
        this.jwtAuthService = jwtAuthService;
        this.emailService = emailService;
        this.mapper = mapper;
    }

    [HttpPost("login")]
    [Produces(typeof(LoginResult))]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var user = await userManager.FindByNameAsync(loginModel.Login)
            ?? await userManager.FindByEmailAsync(loginModel.Login);

        if (user == null || !await userManager.CheckPasswordAsync(user, loginModel.Password))
            return NotFound();

        if (!user.EmailConfirmed)
            return Unauthorized(new { Messge = "Email not confirmed" });

        var roles = await userManager.GetRolesAsync(user);
        var claims = CreateClaims(user.UserName, roles);

        var (jwtSecurityToken, refreshToken) = jwtAuthService.GenerateTokens(user.UserName, claims, DateTime.Now);

        var userView = mapper.Map<UserViewModel>(user);
        var tokens = new TokensViewModel(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            jwtSecurityToken.ValidTo, refreshToken.TokenString);
        
        return Ok(new LoginResult(
            tokens,
            userView,
            roles
        ));
    }

    [Authorize]
    [HttpPost("logout")]
    public ActionResult Logout()
    {
        var userName = currentUserService.GetCurrentUserName();

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
            return Conflict("User already exists");

        var user = mapper.Map<UserEntity>(registerModel);
        
        var result = await userManager.CreateAsync(user, registerModel.Password);

        await userManager.AddToRoleAsync(user, UserRoles.User);

        if (!result.Succeeded)
            return BadRequest("User creation failed");

        await SendEmailConfirmationMessageAsync(user);

        return Ok();
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
    {
        if (changePasswordModel.OldPassword == changePasswordModel.NewPassword)
            return UnprocessableEntity("Passwords should be different");

        var user = await currentUserService.GetCurrentUser();

        if (user == null)
            return NotFound();

        var changePasswordResult = await userManager.ChangePasswordAsync(user,
            changePasswordModel.OldPassword, changePasswordModel.NewPassword);

        if (!changePasswordResult.Succeeded)
            return BadRequest(changePasswordResult.ToString());

        return Ok();
    }

    [HttpPost("email-confirmation-message")]
    public async Task<IActionResult> SendEmailConfirmation(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user == null)
            return NotFound();

        await SendEmailConfirmationMessageAsync(user);

        return Ok();
    }

    [HttpPost("confirm-email", Name = nameof(ConfirmEmail))]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var result = await userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
            return Ok("Email confirmed");
        
        return BadRequest(result.ToString());
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        if (user == null || !await userManager.IsEmailConfirmedAsync(user))
            return NotFound(); // return NotFound to don't discover is email exists or not

        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Link(nameof(ResetPassword), new { userId = user.Id, code });

        await emailService.SendEmailAsync(
            user.Email,
            "Restore password",
            $"Для сброса пароля пройдите по ссылке: <a href='{callbackUrl}'>link</a>");

        return Ok();
    }

    [HttpPost("reset-password", Name = nameof(ResetPassword))]
    public async Task<IActionResult> ResetPassword(string userId, string code, string newPassword)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var result = await userManager.ResetPasswordAsync(user, code, newPassword);
        if (!result.Succeeded)
            return BadRequest(result.ToString());

        return Ok();
    }
    
    private static IEnumerable<Claim> CreateClaims(string userName, IEnumerable<string> roles)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        authClaims.AddRange(roles
            .Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        return authClaims;
    }

    private async Task SendEmailConfirmationMessageAsync(UserEntity user)
    {
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var clientAddress = GetClientAddress();
        
        if (clientAddress is null)
            return;

        var callbackUrl = Url.Link(clientAddress + "/confirm", new { userId = user.Id, code });

        await emailService.SendEmailAsync(
            user.Email,
            "Подтверждение регистрации",
            $"Подтвердите регистрацию, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");
    }

    private string? GetClientAddress()
    {
        var origin = HttpContext.Request.Headers.Origin;
        var referer = HttpContext.Request.Headers.Referer;

        return origin.FirstOrDefault() ?? referer.FirstOrDefault();
    }
}