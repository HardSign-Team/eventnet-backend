using System.ComponentModel.DataAnnotations;
using Eventnet.DataAccess.Models;

namespace Eventnet.Api.Models.Authentication;

public class RegisterModel
{
    [Required] public string UserName { get; init; } = null!;

    [EmailAddress] [Required] public string Email { get; init; } = null!;
    
    [Phone] public string? PhoneNumber { get; init; } = null!;

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Password and ConfirmPassword must match.")]
    [Required]
    public string ConfirmPassword { get; init; } = null!;

    [DataType(DataType.Password)]
    [Required]
    public string Password { get; init; } = null!;
    
    [Required]
    public Gender Gender { get; init; }
    
    [Required]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public DateTime BirthDate { get; init; }
}