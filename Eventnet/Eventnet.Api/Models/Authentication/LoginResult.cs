using Eventnet.DataAccess;
using Eventnet.Models.Authentication.Tokens;

namespace Eventnet.Models.Authentication;

public record LoginResult(TokensViewModel Tokens, UserViewModel User, IList<string> UserRoles);