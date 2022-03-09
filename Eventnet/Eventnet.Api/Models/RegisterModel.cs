using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models;

public record RegisterModel(
    [Required] string UserName,
    [Required] [EmailAddress] string Email,
    [Required] [DataType(DataType.Password)]
    string Password,
    [Phone] string? Phone);