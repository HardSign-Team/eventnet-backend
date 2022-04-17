#pragma warning disable CS8618
// Used for Configuration
namespace Eventnet.Api.Models.Authentication.Tokens;

public class JwtTokenConfig
{
    public int AccessTokenExpiration { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int RefreshTokenExpiration { get; set; }
    public string Secret { get; set; }
}