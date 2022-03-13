namespace Eventnet.Models;

public record LoginModel(
    string? UserName,
    string? Email,
    string Password);