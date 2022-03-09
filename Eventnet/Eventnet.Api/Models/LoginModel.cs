using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models;

public record LoginModel(
    string? Username,
    string? Email,
    [Required] string Password);