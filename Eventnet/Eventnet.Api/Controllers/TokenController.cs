using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Eventnet.Api.Models;
using Eventnet.Api.Models.Authentication.Tokens;
using Eventnet.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Eventnet.Api.Controllers;

[Route("api/token")]
public class TokenController : Controller
{
    private readonly IJwtAuthService jwtAuthService;
    private readonly IMapper mapper;
    private readonly CurrentUserService currentUserService;

    public TokenController(
        CurrentUserService currentUserService,
        IJwtAuthService jwtAuthService,
        IMapper mapper)
    {
        this.currentUserService = currentUserService;
        this.jwtAuthService = jwtAuthService;
        this.mapper = mapper;
    }

    [Authorize]
    [HttpGet("me")]
    [Produces(typeof(UserViewModel))]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await currentUserService.GetCurrentUserAsync();

        if (user == null)
            return NotFound();

        return Ok(mapper.Map<UserViewModel>(user));
    }

    [HttpPost("refresh-token")]
    [Produces(typeof(TokensViewModel))]
    public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.RefreshToken))
            return Unauthorized();
        try
        {
            var (jwtSecurityToken, (_, tokenString, _)) =
                jwtAuthService.Refresh(request.RefreshToken, request.AccessToken, DateTime.Now);

            return Ok(new TokensViewModel(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                jwtSecurityToken.ValidTo,
                tokenString));
        }
        catch (SecurityTokenException e)
        {
            return Unauthorized(e.Message);
        }
    }
}