using Eventnet.Api.Models.Authentication.Tokens;

namespace Eventnet.Api.Models.Authentication;

public record LoginResult(TokensViewModel Tokens, UserViewModel User, IList<string> UserRoles);
