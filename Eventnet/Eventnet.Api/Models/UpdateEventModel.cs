using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models;

public record UpdateEventModel(
    string Username,
    [EmailAddress] string Email,
    [DataType(DataType.Password)]
    string Password,
    [Phone] string? Phone
);