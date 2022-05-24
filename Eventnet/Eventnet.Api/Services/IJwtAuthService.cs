using System.Security.Claims;
using Eventnet.Api.Models.Authentication.Tokens;

namespace Eventnet.Api.Services;

public interface IJwtAuthService
{
    JwtAuthResult GenerateTokens(string userName, DateTime now);
    JwtAuthResult Refresh(string refreshToken, DateTime now);
    void RemoveRefreshToken(string refreshToken);
}