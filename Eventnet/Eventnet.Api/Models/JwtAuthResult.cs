using System.IdentityModel.Tokens.Jwt;

namespace Eventnet.Models;

public record JwtAuthResult(JwtSecurityToken AccessToken);