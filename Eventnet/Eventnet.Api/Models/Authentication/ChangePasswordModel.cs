using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models.Authentication;

public record ChangePasswordModel(
    [DataType(DataType.Password)] string OldPassword,
    [DataType(DataType.Password)] string NewPassword);