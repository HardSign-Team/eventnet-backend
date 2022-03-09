using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Eventnet.Models;
using Microsoft.IdentityModel.Tokens;

namespace Eventnet.Services;

public record RefreshToken(string UserName, string TokenString, DateTime ExpireAt);

public class JwtAuthService : IJwtAuthService
{
    private readonly ConcurrentDictionary<string, RefreshToken> usersRefreshTokens = new();

    private readonly JwtTokenConfig jwtTokenConfig;
    private readonly byte[] secret;
    private readonly TokenValidationParameters tokenValidationParameters;

    public JwtAuthService(JwtTokenConfig jwtTokenConfig)
    {
        this.jwtTokenConfig = jwtTokenConfig;
        secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
        tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtTokenConfig.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secret),
            ValidAudience = jwtTokenConfig.Audience,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    }

    public JwtAuthResult GenerateTokens(string username, IEnumerable<Claim> claims, DateTime now)
    {
        var jwtToken = new JwtSecurityToken(
            jwtTokenConfig.Issuer,
            jwtTokenConfig.Audience,
            claims,
            expires: now.AddMinutes(jwtTokenConfig.AccessTokenExpiration),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature));

        var refreshToken = new RefreshToken(
            username,
            GenerateRefreshTokenString(),
            now.AddMinutes(jwtTokenConfig.RefreshTokenExpiration));

        usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken,
            (_, _) => refreshToken);

        return new JwtAuthResult(jwtToken, refreshToken);
    }

    public JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now)
    {
        var (principal, jwtToken) = DecodeJwtToken(accessToken);

        if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            throw new SecurityTokenException("Invalid token");

        var userName = principal.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        if (!usersRefreshTokens.TryGetValue(refreshToken, out var existingRefreshToken))
            throw new SecurityTokenException("Invalid token");

        if (existingRefreshToken.UserName != userName || existingRefreshToken.ExpireAt < now)
            throw new SecurityTokenException("Invalid token");

        return GenerateTokens(userName, principal.Claims.ToArray(), now);
    }

    private (ClaimsPrincipal principal, JwtSecurityToken?) DecodeJwtToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new SecurityTokenException("Invalid token");

        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token,
                tokenValidationParameters,
                out var validatedToken);

        return (principal, validatedToken as JwtSecurityToken);
    }

    private static string GenerateRefreshTokenString()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}