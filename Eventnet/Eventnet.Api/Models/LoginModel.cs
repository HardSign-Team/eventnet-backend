using System.ComponentModel.DataAnnotations;

namespace Eventnet.Api.Models;

public record LoginModel(
    [Required] string Username,
    [Required] string Password);