using Eventnet.DataAccess.Models;

namespace Eventnet.Api.Models;

public record UserViewModel(
    string UserName,
    string Email,
    Gender Gender,
    DateTime BirthDate);