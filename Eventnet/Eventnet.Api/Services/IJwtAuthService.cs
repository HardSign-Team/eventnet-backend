using System.Security.Claims;
using Eventnet.Models;

namespace Eventnet.Services;

public interface IJwtAuthService
{
    JwtAuthResult GenerateTokens(string username, IEnumerable<Claim> claims, DateTime now);
    JwtAuthResult Refresh(string refreshToken, string accessToken, DateTime now);
    void RemoveRefreshTokenByUserName(string userName);
}