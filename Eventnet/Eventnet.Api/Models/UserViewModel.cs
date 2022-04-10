using Eventnet.Api.Models.Authentication;

namespace Eventnet.Api.Models;

public record UserViewModel(
    string UserName,
    string Email,
    Gender Gender,
    DateTime BirthDate);