using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Eventnet.Models;
using Eventnet.Services;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace Eventnet.Api.Tests;

public class JwtAuthService_Should
{
    private const string ValidJwtToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9." +
        "eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJleHAiOjE2NDY4NTA3MTUsImlzcyI6InRlc3QiLCJhdWQiOiJ0ZXN0In0." +
        "NnTZma9Wm-u9DilwLGApGT44Bh5e-Bc6tWmkyqr7uWs";
    
    private static readonly JwtTokenConfig DefaultConfig = new()
    {
        AccessTokenExpiration = 60,
        Audience = "test",
        Issuer = "test",
        RefreshTokenExpiration = 60,
        Secret = "I see a red door and I want it painted black",
    };

    private JwtAuthService jwtAuthService;
    private DateTime now;

    [SetUp]
    public void Setup()
    {
        jwtAuthService = new JwtAuthService(DefaultConfig);
        now = DateTime.Now;
    }

    [Test]
    public void Generate_Access_Token_With_Config_Data()
    {
        const string userName = "admin";
        var claims = new[] { new Claim(ClaimTypes.Name, userName) };
        
        var (jwtSecurityToken, _) = jwtAuthService.GenerateTokens(userName, claims, now);

        jwtSecurityToken.Issuer.Should().Be(DefaultConfig.Issuer);
        jwtSecurityToken.Audiences.Should().Contain(DefaultConfig.Audience);
    }

    [Test]
    public void Generate_Access_Token_With_Claims()
    {
        const string userName = "user";
        var testClaim = new Claim("TestClaimName", userName);
        var claims = new[] { testClaim };

        var (jwtSecurityToken, _) = jwtAuthService.GenerateTokens(userName, claims, now);

        jwtSecurityToken.Claims.Select(x => x.Type).Should().Contain(testClaim.Type);
        jwtSecurityToken.Claims.Select(x => x.Value).Should().Contain(testClaim.Value);
    }

    [Test]
    public void Generate_Different_Tokens_For_Different_Users()
    {
        const string userName1 = "user1";
        var claims1 = new[] { new Claim(ClaimTypes.Name, userName1) };
        const string userName2 = "user2";
        var claims2 = new[] { new Claim(ClaimTypes.Name, userName2) };

        var tokens1 = jwtAuthService.GenerateTokens(userName1, claims1, now);
        var tokens2 = jwtAuthService.GenerateTokens(userName2, claims2, now);
        var accessToken1 = new JwtSecurityTokenHandler().WriteToken(tokens1.AccessToken);
        var accessToken2 = new JwtSecurityTokenHandler().WriteToken(tokens2.AccessToken);

        accessToken1.Should().NotBe(accessToken2);
        tokens1.RefreshToken.TokenString.Should().NotBe(tokens2.RefreshToken.TokenString);
    }

    [Test]
    public void Rotate_Refresh_Token()
    {
        const string userName = "admin";
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName)
        };

        var tokens1 = jwtAuthService.GenerateTokens(userName, claims, now);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokens1.AccessToken);
        var refreshedToken = jwtAuthService.Refresh(tokens1.RefreshToken.TokenString, accessToken, now);

        refreshedToken.RefreshToken.TokenString.Should().NotBe(tokens1.RefreshToken.TokenString);
    }
    
    [Test]
    public void Update_Access_Token_After_Refresh()
    {
        const string userName = "admin";
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName)
        };

        var tokens1 = jwtAuthService.GenerateTokens(userName, claims, now);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokens1.AccessToken);
        var refreshedToken = jwtAuthService.Refresh(tokens1.RefreshToken.TokenString, accessToken, now);
        var accessToken2 = new JwtSecurityTokenHandler().WriteToken(refreshedToken.AccessToken);

        accessToken2.Should().NotBe(accessToken);
    }

    [Test]
    public void Throw_Exception_On_Invalid_Access_Token()
    {
        const string userName = "admin";
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName)
        };
        
        var tokens = jwtAuthService.GenerateTokens(userName, claims, now);
        
        Assert.Throws<SecurityTokenException>(
            () => jwtAuthService.Refresh(tokens.RefreshToken.TokenString, "", now));
    }
    
    [Test]
    public void Throw_Exception_When_Refresh_Before_Generate()
    {
        const string userName = "admin";
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName)
        };
        
        Assert.Throws<SecurityTokenException>(
            () => jwtAuthService.Refresh("test refresh", ValidJwtToken, now));
    }
    
    [Test]
    public void Throw_Exception_When_Refresh_With_Other_UserName()
    {
        const string userName = "admin";
        var claims = Array.Empty<Claim>();
        
        var tokens1 = jwtAuthService.GenerateTokens(userName, claims, now);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokens1.AccessToken);
        
        Assert.Throws<SecurityTokenException>(
            () => jwtAuthService.Refresh(tokens1.RefreshToken.TokenString, accessToken, now));
    }
    
    [Test]
    public void Throw_Exception_When_Token_Is_Expired()
    {
        const string userName = "admin";
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName)
        };
        
        var tokens1 = jwtAuthService.GenerateTokens(userName, claims, now);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokens1.AccessToken);
        
        Assert.Throws<SecurityTokenException>(
            () => jwtAuthService.Refresh(tokens1.RefreshToken.TokenString, accessToken, now + TimeSpan.FromHours(2)));
    }

    [Test]
    public void Remove_Refresh_Token_By_UserName()
    {
        const string userName = "admin";
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName)
        };
        
        var tokens1 = jwtAuthService.GenerateTokens(userName, claims, now);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokens1.AccessToken);

        jwtAuthService.RemoveRefreshTokenByUserName(userName);
        
        Assert.Throws<SecurityTokenException>(
            () => jwtAuthService.Refresh(tokens1.RefreshToken.TokenString, accessToken, now));
    }
}