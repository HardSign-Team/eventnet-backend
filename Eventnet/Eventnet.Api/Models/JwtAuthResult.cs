using System.IdentityModel.Tokens.Jwt;
using Eventnet.Services;

namespace Eventnet.Models;

public record JwtAuthResult(JwtSecurityToken AccessToken, RefreshToken RefreshToken);