using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Eventnet.Models;
using Microsoft.IdentityModel.Tokens;

namespace Eventnet.Services;

public class JwtAuthService : IJwtAuthService
{
    private readonly JwtTokenConfig jwtTokenConfig;
    private readonly byte[] secret;

    public JwtAuthService(JwtTokenConfig jwtTokenConfig)
    {
        this.jwtTokenConfig = jwtTokenConfig;
        secret = Encoding.ASCII.GetBytes(jwtTokenConfig.Secret);
    }

    public JwtAuthResult GenerateTokens(Claim[] claims, DateTime now)
    {
        var jwtToken = new JwtSecurityToken(
            jwtTokenConfig.Issuer,
            jwtTokenConfig.Audience,
            claims,
            expires: now.AddMinutes(jwtTokenConfig.AccessTokenExpiration),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature));

        return new JwtAuthResult
        {
            AccessToken = jwtToken
        };
    }
}