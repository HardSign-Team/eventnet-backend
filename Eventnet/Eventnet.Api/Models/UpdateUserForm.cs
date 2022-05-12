using Eventnet.DataAccess.Models;

namespace Eventnet.Api.Models;

public class UpdateUserForm
{
    public string UserName { get; init; } = null!;
    public DateTime BirthDate { get; init; }
    public Gender Gender { get; init; }
    public string? PhoneNumber { get; init; } = null!;
}