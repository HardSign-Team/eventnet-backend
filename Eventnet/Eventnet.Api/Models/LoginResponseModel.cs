using Eventnet.DataAccess;

namespace Eventnet.Models;

public record LoginResponseModel(string AccessToken, DateTime ExpiredAt, ApplicationUser User, IList<string> UserRoles);