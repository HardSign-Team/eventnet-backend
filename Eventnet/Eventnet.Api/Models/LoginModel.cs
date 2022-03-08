using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models;

public record LoginModel(    
    [Required] string Username,
    [Required] string Password);