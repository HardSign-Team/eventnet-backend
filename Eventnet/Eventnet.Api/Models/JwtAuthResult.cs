using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace Eventnet.Models;

public record JwtAuthResult([property: JsonPropertyName("accessToken")] JwtSecurityToken AccessToken);