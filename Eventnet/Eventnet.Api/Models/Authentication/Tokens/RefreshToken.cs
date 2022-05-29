namespace Eventnet.Api.Models.Authentication.Tokens;

public record RefreshToken(string TokenString, DateTime ExpireAt);