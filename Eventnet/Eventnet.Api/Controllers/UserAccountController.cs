using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Eventnet.Api.Models;
using Eventnet.Api.Models.Authentication;
using Eventnet.Api.Models.Authentication.Tokens;
using Eventnet.Api.Services;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Api.Controllers;

[Route("api/auth")]
public class UserAccountController : Controller
{
    private readonly UserManager<UserEntity> userManager;
    private readonly CurrentUserService currentUserService;
    private readonly IJwtAuthService jwtAuthService;
    private readonly IEmailService emailService;
    private readonly IMapper mapper;
    private readonly IForgotPasswordService forgotPasswordService;

    public UserAccountController(
        UserManager<UserEntity> userManager,
        CurrentUserService currentUserService,
        IJwtAuthService jwtAuthService,
        IEmailService emailService,
        IMapper mapper,
        IForgotPasswordService forgotPasswordService)
    {
        this.userManager = userManager;
        this.currentUserService = currentUserService;
        this.jwtAuthService = jwtAuthService;
        this.emailService = emailService;
        this.mapper = mapper;
        this.forgotPasswordService = forgotPasswordService;
    }

    /// <summary>
    ///     Logins user
    /// </summary>
    /// <param name="loginModel">Login may be a userName or an email</param>
    /// <returns></returns>
    [HttpPost("login")]
    [Produces(typeof(LoginResult))]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var user = await userManager.FindByNameAsync(loginModel.Login)
            ?? await userManager.FindByEmailAsync(loginModel.Login);

        var passwordCorrect = await userManager.CheckPasswordAsync(user, loginModel.Password);
        if (user is null || !passwordCorrect)
            return NotFound();

        if (!user.EmailConfirmed)
            return Unauthorized(new { Messge = "Email not confirmed" });

        var roles = await userManager.GetRolesAsync(user);
        var claims = CreateClaims(user.UserName, roles);

        var (jwtSecurityToken, refreshToken) = jwtAuthService.GenerateTokens(user.UserName, claims, DateTime.Now);

        var userView = mapper.Map<UserViewModel>(user);
        var tokens = new TokensViewModel(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            jwtSecurityToken.ValidTo,
            refreshToken.TokenString);

        return Ok(new LoginResult(tokens,
            userView,
            roles));
    }

    [Authorize]
    [HttpPost("logout")]
    public ActionResult Logout()
    {
        var userName = currentUserService.GetCurrentUserName();

        if (userName is null)
            return Unauthorized();

        jwtAuthService.RemoveRefreshTokenByUserName(userName);
        return Ok();
    }

    /// <summary>
    ///     Register user and send message with confirmation link to an email.
    ///     Link is "OriginHeader" + "/confirm"
    /// </summary>
    /// <param name="registerModel"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var userNameExists = await userManager.FindByNameAsync(registerModel.UserName);

        if (userNameExists is not null)
            return Conflict(nameof(registerModel.UserName));

        var emailExists = await userManager.FindByEmailAsync(registerModel.Email);

        if (emailExists is not null)
            return Conflict(nameof(registerModel.Email));

        var user = mapper.Map<UserEntity>(registerModel);

        var result = await userManager.CreateAsync(user, registerModel.Password);

        if (!result.Succeeded)
            return BadRequest("User creation failed");

        await SendEmailConfirmationMessageAsync(user);

        return Ok();
    }

    /// <summary>
    ///     Changes user password.
    ///     Password mustn't match
    /// </summary>
    /// <param name="changePasswordModel"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel changePasswordModel)
    {
        if (changePasswordModel.OldPassword == changePasswordModel.NewPassword)
            return UnprocessableEntity("Passwords should be different");

        var user = await currentUserService.GetCurrentUserAsync();

        if (user is null)
            return NotFound();

        var changePasswordResult = await userManager.ChangePasswordAsync(user,
            changePasswordModel.OldPassword,
            changePasswordModel.NewPassword);

        if (!changePasswordResult.Succeeded)
            return BadRequest(changePasswordResult.ToString());

        return Ok();
    }

    /// <summary>
    ///     Just sends email confirmation message.
    ///     Link is "OriginHeader" + "/confirm"
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    [HttpPost("email-confirmation-message")]
    public async Task<IActionResult> SendEmailConfirmation(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user is null)
            return NotFound();

        await SendEmailConfirmationMessageAsync(user);

        return Ok();
    }

    /// <summary>
    ///     Verify that code is right and confirm email
    /// </summary>
    /// <param name="userId">Id from email redirect link</param>
    /// <param name="code">Code from email redirect link</param>
    /// <returns></returns>
    [HttpPost("confirm-email", Name = nameof(ConfirmEmail))]
    public async Task<IActionResult> ConfirmEmail(Guid userId, string code)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user is null)
            return NotFound();

        var result = await userManager.ConfirmEmailAsync(user, code);

        if (!result.Succeeded)
            return BadRequest(result.ToString());

        return Ok("Email Confirmed");
    }

    /// <summary>
    ///     Sends email message with code to user's mail
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("password/forgot")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);

        var emailConfirmed = await userManager.IsEmailConfirmedAsync(user);
        if (user is null || !emailConfirmed)
            return NotFound(); // return NotFound to don't discover is email exists or not

        await forgotPasswordService.SendCodeAsync(user.Email);

        return Ok();
    }

    /// <summary>
    ///     Verify that code is exists and belongs to user
    /// </summary>
    /// <param name="email"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet("password/forgot/code")]
    [Produces(typeof(VerifyPasswordCodeResultModel))]
    public IActionResult VerifyUserCode(string email, string code)
    {
        return Ok(new VerifyPasswordCodeResultModel(forgotPasswordService.VerifyCode(email, code)));
    }

    /// <summary>
    ///     Remove user's password and set a new one
    /// </summary>
    /// <param name="restorePasswordModel"></param>
    /// <returns></returns>
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPassword(RestorePasswordModel restorePasswordModel)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var user = await userManager.FindByEmailAsync(restorePasswordModel.Email);
        if (user is null)
            return NotFound();

        if (!forgotPasswordService.VerifyCode(restorePasswordModel.Email, restorePasswordModel.Code))
            return BadRequest();

        await userManager.RemovePasswordAsync(user);
        var result = await userManager.AddPasswordAsync(user, restorePasswordModel.NewPassword);

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
        var clientAddress = GetClientAddress() + $"completed-register/{user.Id}/{code}";

        await emailService.SendEmailAsync(user.Email,
            "Подтверждение регистрации",
            $"Подтвердите регистрацию, перейдя по ссылке: <a href='{clientAddress}'>{clientAddress}</a>");
    }

    private string GetClientAddress() => Request.Headers["Referer"].ToString();
}