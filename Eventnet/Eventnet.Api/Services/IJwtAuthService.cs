using System.Security.Claims;
using Eventnet.Api.Models;

namespace Eventnet.Api.Services;

public interface IJwtAuthService
{
    JwtAuthResult GenerateTokens(Claim[] claims, DateTime now);
}