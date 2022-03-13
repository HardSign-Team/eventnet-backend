
using Eventnet.DataAccess;

namespace Eventnet.Models;

public record LoginResponseModel(string AccessToken, DateTime ExpiredAt, UserEntity UserEntity,
    IList<string> UserRoles);