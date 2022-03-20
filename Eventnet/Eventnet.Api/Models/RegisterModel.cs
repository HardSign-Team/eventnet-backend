using System.ComponentModel.DataAnnotations;

namespace Eventnet.Api.Models;

public record RegisterModel(
    [Required] string Username,
    [Required] [EmailAddress] string Email,
    [Required] [DataType(DataType.Password)]
    string Password,
    [Phone] string? Phone);