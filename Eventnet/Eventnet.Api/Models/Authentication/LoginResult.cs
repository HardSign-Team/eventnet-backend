using Eventnet.DataAccess;

namespace Eventnet.Models.Authentication;

public record LoginResult(string AccessToken, DateTime ExpiredAt,
    string RefreshToken, UserEntity User, IList<string> UserRoles);