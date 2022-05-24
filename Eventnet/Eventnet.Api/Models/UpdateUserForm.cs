using Eventnet.DataAccess.Models;

namespace Eventnet.Api.Models;

public record UpdateUserForm(string UserName, DateTime BirthDate, Gender Gender);