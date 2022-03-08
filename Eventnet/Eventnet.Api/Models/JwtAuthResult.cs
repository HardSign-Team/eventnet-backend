using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace Eventnet.Models;

public class JwtAuthResult
{
    [JsonPropertyName("accessToken")] 
    public JwtSecurityToken AccessToken { get; init; }
}