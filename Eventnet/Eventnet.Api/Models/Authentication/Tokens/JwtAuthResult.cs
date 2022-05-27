namespace Eventnet.Api.Models.Authentication.Tokens;

public record JwtAuthResult(AccessToken AccessToken, RefreshToken RefreshToken);