﻿using System.IdentityModel.Tokens.Jwt;
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

[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IJwtAuthService jwtAuthService;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly UserManager<ApplicationUser> userManager;

    public AuthController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtAuthService jwtAuthService)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        var userExists = await userManager.FindByNameAsync(registerModel.UserName);

        if (userExists != null)
            return BadRequest("User already exists");

        var user = new ApplicationUser
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

        // TODO: replace with CreatedAtAction when implement UserController 
        return Ok(new RegisterResult("User created successfully", user));
    }
}