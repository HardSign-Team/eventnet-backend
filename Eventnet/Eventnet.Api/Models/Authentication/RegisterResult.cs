using Eventnet.DataAccess;

namespace Eventnet.Models.Authentication;

public record RegisterResult(string Status, UserEntity User);