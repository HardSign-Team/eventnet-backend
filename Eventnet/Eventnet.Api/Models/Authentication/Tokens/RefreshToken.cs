namespace Eventnet.Api.Models.Authentication.Tokens;

public record RefreshToken(string UserName, string TokenString, DateTime ExpireAt);