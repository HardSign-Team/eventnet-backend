using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Eventnet.Api.Models.Authentication.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace Eventnet.Api.Services;

public record RefreshTokenInfo(string Id, string UserName, DateTime ExpiresAt)
{
    public bool Revoked { get; set; }
}

public class JwtAuthService : IJwtAuthService
{
    private readonly Dictionary<string, RefreshTokenInfo> usersRefreshTokens = new();

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

    public JwtAuthResult GenerateTokens(string userName, DateTime now)
    {
        var valid = now.AddMinutes(jwtTokenConfig.AccessTokenExpiration);
        var accessClaims = new List<Claim> { new(ClaimTypes.Name, userName) };
        var accessToken = new JwtSecurityToken(jwtTokenConfig.Issuer,
            jwtTokenConfig.Audience,
            accessClaims.ToList(),
            expires: valid,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature));

        var validTo = now.AddMinutes(jwtTokenConfig.RefreshTokenExpiration);
        var refreshToken = GenerateRefreshToken(Guid.NewGuid().ToString(), userName, validTo);

        return new JwtAuthResult(
            new AccessToken(new JwtSecurityTokenHandler().WriteToken(accessToken), valid),
            new RefreshToken(new JwtSecurityTokenHandler().WriteToken(refreshToken), validTo));
    }

    public JwtAuthResult Refresh(string refreshToken, DateTime now)
    {
        var jwtToken = DecodeJwtToken(refreshToken);

        if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            throw new SecurityTokenException("Invalid token");
        
        if(jwtToken.ValidTo < now.ToUniversalTime())
            throw new SecurityTokenException("Token expired");

        var id = jwtToken.Id;
        var userName = usersRefreshTokens[id].UserName;
        var revokedUserToken = usersRefreshTokens.Values
            .FirstOrDefault(x => x.Id == id && x.Revoked);
        if (revokedUserToken is not null)
        {
            RevokeAllUserTokens(userName);
            throw new SecurityTokenException("Forgery");
        }

        var token = usersRefreshTokens.Values.FirstOrDefault(x => x.Id == id && !x.Revoked);
        if(token is null)
            throw new SecurityTokenException("Not logged in");

        token.Revoked = true;
        
        return GenerateTokens(userName, now);
    }

    public void RemoveRefreshToken(string refreshToken)
    {
        var jwtToken = DecodeJwtToken(refreshToken);
        if (usersRefreshTokens.ContainsKey(jwtToken.Id))
            usersRefreshTokens[jwtToken.Id].Revoked = true;
    }

    private void RevokeAllUserTokens(string userName)
    {
        var tokens = usersRefreshTokens
            .Where(x => x.Value.UserName == userName && !x.Value.Revoked);

        foreach (var token in tokens)
        {
            token.Value.Revoked = true;
            usersRefreshTokens[token.Key] = token.Value;
        }
    }

    private JwtSecurityToken GenerateRefreshToken(string id, string userName, DateTime expireAt)
    {
        var claims = new List<Claim> { new(JwtRegisteredClaimNames.Jti, id) };
        
        var token = new JwtSecurityToken(jwtTokenConfig.Issuer,
            jwtTokenConfig.Audience,
            claims,
            expires: expireAt,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature));
        
        var existsToken = usersRefreshTokens.Values
            .FirstOrDefault(x => x.UserName == userName && !x.Revoked);
        if (existsToken is not null)
            existsToken.Revoked = true;
        
        var newToken = new RefreshTokenInfo(id,
            userName,
            expireAt);

        usersRefreshTokens.Add(id, newToken);

        return token;
    }

    private JwtSecurityToken? DecodeJwtToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new SecurityTokenException("Invalid token");

        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token,
                tokenValidationParameters,
                out var validatedToken);

        return validatedToken as JwtSecurityToken;
    }
}