using System.IdentityModel.Tokens.Jwt;

namespace Eventnet.Api.Models;

public record JwtAuthResult(JwtSecurityToken AccessToken);