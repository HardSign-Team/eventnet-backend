using System.IdentityModel.Tokens.Jwt;
using Eventnet.Api.Services;

namespace Eventnet.Api.Models.Authentication.Tokens;

public record JwtAuthResult(AccessToken AccessToken, RefreshToken RefreshToken);