using Eventnet.DataAccess;

namespace Eventnet.Models;

public record LoginResponseModel(string AccessToken, DateTime ExpiredAt,
    string RefreshToken, ApplicationUser User, IList<string> UserRoles);