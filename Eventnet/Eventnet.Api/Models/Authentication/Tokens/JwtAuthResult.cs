using System.IdentityModel.Tokens.Jwt;
using Eventnet.Services;

namespace Eventnet.Models.Authentication.Tokens;

public record JwtAuthResult(JwtSecurityToken AccessToken, RefreshToken RefreshToken);