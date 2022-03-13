using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Eventnet.DataAccess;
using Eventnet.Models;
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

    public TokenController(UserManager<UserEntity> userManager,
        IJwtAuthService jwtAuthService)
    {
        this.userManager = userManager;
        this.jwtAuthService = jwtAuthService;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userName = User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        if (userName == null)
            return Unauthorized();

        var user = await userManager.FindByNameAsync(userName);

        return Ok(user); // TODO: return business model 
    }

    [HttpPost("refresh-token")]
    [Produces(typeof(LoginResult))]
    [Authorize]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var userName = User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            if (userName == null)
                return Unauthorized();

            var user = await userManager.FindByNameAsync(userName);
            var userRoles = await userManager.GetRolesAsync(user);

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Unauthorized();

            var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");

            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized();

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