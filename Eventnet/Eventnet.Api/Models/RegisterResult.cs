using Eventnet.DataAccess;

namespace Eventnet.Models;

public record RegisterResult(string Status, UserEntity User);