using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.Models;

public record LoginResponseModel(string AccessToken, DateTime ExpiredAt, UserEntity UserEntity,
    IList<string> UserRoles);