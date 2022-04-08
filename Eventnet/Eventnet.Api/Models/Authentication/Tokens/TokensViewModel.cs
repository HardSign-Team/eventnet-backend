namespace Eventnet.Models.Authentication.Tokens;

public record TokensViewModel(string AccessToken, DateTime ExpiredAt, string RefreshToken);