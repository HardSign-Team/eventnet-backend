namespace Eventnet.Api.Models.Authentication.Tokens;

public record AccessToken(string TokenString, DateTime ExpireAt);