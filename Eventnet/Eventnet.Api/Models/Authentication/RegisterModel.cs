using System.ComponentModel.DataAnnotations;
using Eventnet.DataAccess.Models;

namespace Eventnet.Api.Models.Authentication;

public class RegisterModel
{
    [Required]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime BirthDate { get; init; }

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Password and ConfirmPassword must match.")]
    [Required]
    public string ConfirmPassword { get; init; } = null!;

    [EmailAddress] [Required] public string Email { get; init; } = null!;

    [Required] public Gender Gender { get; init; }

    [DataType(DataType.Password)]
    [Required]
    public string Password { get; init; } = null!;

    [Phone] public string? PhoneNumber { get; init; } = null!;
    [Required] public string UserName { get; init; } = null!;
}