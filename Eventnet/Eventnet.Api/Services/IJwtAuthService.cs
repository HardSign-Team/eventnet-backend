using System.Security.Claims;
using Eventnet.Models.Authentication.Tokens;

namespace Eventnet.Services;

public interface IJwtAuthService
{
    JwtAuthResult GenerateTokens(string userName, IEnumerable<Claim> claims, DateTime now);
    JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now);
    void RemoveRefreshTokenByUserName(string userName);
}