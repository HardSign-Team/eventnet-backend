using System.Security.Claims;
using Eventnet.Models;

namespace Eventnet.Services;

public interface IJwtAuthService
{
    JwtAuthResult GenerateTokens(Claim[] claims, DateTime now);
}