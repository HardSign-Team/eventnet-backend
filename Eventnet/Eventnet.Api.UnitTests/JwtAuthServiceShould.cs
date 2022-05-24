using System;
using Eventnet.Api.Models.Authentication.Tokens;
using Eventnet.Api.Services;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

// TESTS
#pragma warning disable CS8618

namespace Eventnet.Api.UnitTests;

public class JwtAuthServiceShould
{
    private static readonly JwtTokenConfig DefaultConfig = new()
    {
        AccessTokenExpiration = 60,
        Audience = "test",
        Issuer = "test",
        RefreshTokenExpiration = 60,
        Secret = "I see a red door and I want it painted black"
    };

    private JwtAuthService sut = null!;
    private DateTime now;

    [SetUp]
    public void Setup()
    {
        sut = new JwtAuthService(DefaultConfig);
        now = DateTime.Now;
    }

    [Test]
    public void Generate_Access_Token_With_Config_Data()
    {
        const string userName = "admin";

        var (accessToken, _) = sut.GenerateTokens(userName, now);

        accessToken.ExpireAt.Should().Be(now.AddMinutes(DefaultConfig.AccessTokenExpiration));
    }

    [Test]
    public void Generate_Different_Tokens_For_Different_Users()
    {
        const string userName1 = "user1";
        const string userName2 = "user2";

        var (jwtSecurityToken1, refreshToken1) = sut.GenerateTokens(userName1, now);
        var (jwtSecurityToken2, refreshToken2) = sut.GenerateTokens(userName2, now);

        jwtSecurityToken1.TokenString.Should().NotBe(jwtSecurityToken2.TokenString);
        refreshToken1.TokenString.Should().NotBe(refreshToken2.TokenString);
    }

    [Test]
    public void Rotate_Refresh_Token()
    {
        const string userName = "admin";

        var (_, refreshToken) = sut.GenerateTokens(userName, now);
        var refreshedToken = sut.Refresh(refreshToken.TokenString, now);

        refreshedToken.RefreshToken.TokenString.Should().NotBe(refreshToken.TokenString);
    }

    [Test]
    public void Update_Access_Token_After_Refresh()
    {
        const string userName = "admin";

        var (accessToken, refreshToken) = sut.GenerateTokens(userName, now);
        var refreshedToken = sut.Refresh(refreshToken.TokenString, now.AddDays(-10));

        refreshedToken.AccessToken.TokenString.Should().NotBe(accessToken.TokenString);
    }

    [Test]
    public void Throw_Exception_When_Token_Is_Expired()
    {
        const string userName = "admin";

        var (_, refreshToken) = sut.GenerateTokens(userName, now);

        Assert.Throws<SecurityTokenException>(()
            => sut.Refresh(refreshToken.TokenString, now + TimeSpan.FromHours(2)));
    }

    [Test]
    public void Remove_Refresh_Token_By_UserName()
    {
        const string userName = "admin";

        var (_, refreshToken) = sut.GenerateTokens(userName, now);

        sut.RemoveRefreshToken(refreshToken.TokenString);

        Assert.Throws<SecurityTokenException>(() => sut.Refresh(refreshToken.TokenString, now));
    }
}