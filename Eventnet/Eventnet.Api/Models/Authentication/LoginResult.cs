using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.Models.Authentication;

public record LoginResult(string AccessToken, DateTime ExpiredAt,
    string RefreshToken, UserEntity User, IList<string> UserRoles);