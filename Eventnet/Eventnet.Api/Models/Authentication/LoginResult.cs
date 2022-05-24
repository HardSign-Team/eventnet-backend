using Eventnet.Api.Models.Authentication.Tokens;

namespace Eventnet.Api.Models.Authentication;

public record LoginResult(JwtAuthResult Tokens, UserViewModel User, IList<string> UserRoles);