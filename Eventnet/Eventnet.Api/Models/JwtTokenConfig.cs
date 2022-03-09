namespace Eventnet.Models;

public class JwtTokenConfig
{
    public int AccessTokenExpiration { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string Secret { get; set; }
}