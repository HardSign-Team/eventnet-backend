using System.Security.Claims;
using Eventnet.Api.Models.Authentication.Tokens;

namespace Eventnet.Api.Services;

public interface IJwtAuthService
{
    JwtAuthResult GenerateTokens(string userName, IEnumerable<Claim> claims, DateTime now);
    JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now);
    void RemoveRefreshTokenByUserName(string userName);
}