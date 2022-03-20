using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;

namespace Eventnet.Models;

public record LoginResponseModel(string AccessToken, DateTime ExpiredAt, UserEntity UserEntity,
    IList<string> UserRoles);