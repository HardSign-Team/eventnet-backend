namespace Eventnet.Api.Models.Authentication.Tokens;

public record RefreshTokenRequest(string AccessToken, string RefreshToken);