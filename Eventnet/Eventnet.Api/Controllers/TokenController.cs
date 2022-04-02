using System.IdentityModel.Tokens.Jwt;
using Eventnet.DataAccess;
using Eventnet.Models.Authentication;
using Eventnet.Models.Authentication.Tokens;
using Eventnet.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Eventnet.Controllers;

[Route("api/token")]
public class TokenController : Controller
{
    private readonly IJwtAuthService jwtAuthService;
    private readonly UserManager<UserEntity> userManager;
    private readonly CurrentUserService currentUserService;

    public TokenController(UserManager<UserEntity> userManager,
        CurrentUserService currentUserService,
        IJwtAuthService jwtAuthService)
    {
        this.userManager = userManager;
        this.currentUserService = currentUserService;
        this.jwtAuthService = jwtAuthService;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await currentUserService.GetCurrentUser();

        if (user == null)
            return NotFound();

        return Ok(user); // TODO: return business model 
    }

    [HttpPost("refresh-token")]
    [Produces(typeof(LoginResult))]
    [Authorize]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var user = await currentUserService.GetCurrentUser();

        if (user == null)
            return NotFound();

        var userRoles = await userManager.GetRolesAsync(user);

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Unauthorized();

        var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");

        if (string.IsNullOrEmpty(accessToken))
            return Unauthorized();
        try
        {
            var (jwtSecurityToken, (_, tokenString, _)) =
                jwtAuthService.Refresh(request.RefreshToken, accessToken, DateTime.Now);

            return Ok(new LoginResult(
                new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                jwtSecurityToken.ValidTo,
                tokenString,
                user,
                userRoles
            ));
        }
        catch (SecurityTokenException e)
        {
            return Unauthorized(e.Message);
        }
    }
}