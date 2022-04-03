using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models.Authentication;

public class RestorePasswordModel
{
    [Required] public string Code { get; init; } = null!;
    [Required] public string Email { get; init; } = null!;

    [DataType(DataType.Password)]
    [Required]
    public string NewPassword { get; init; } = null!;

    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Password and Confirmation Password must match.")]
    [Required]
    public string NewPasswordConfirm { get; init; } = null!;
}