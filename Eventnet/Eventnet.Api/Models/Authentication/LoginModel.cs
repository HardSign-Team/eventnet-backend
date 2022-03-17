namespace Eventnet.Models.Authentication;

public record LoginModel(
    string? UserName,
    string? Email,
    string Password);