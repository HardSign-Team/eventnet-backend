using System.ComponentModel.DataAnnotations;

namespace Eventnet.Api.Models.Authentication;

public record RegisterModel(
    string UserName,
    [EmailAddress] string Email,
    [DataType(DataType.Password)] string Password,
    [Phone] string? Phone);