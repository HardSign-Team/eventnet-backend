using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.Models.Authentication;

public record RegisterResult(string Status, UserEntity User);