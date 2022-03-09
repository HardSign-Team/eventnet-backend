using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models;

public record LoginModel(
    string? UserName,
    string? Email,
    [Required] string Password);