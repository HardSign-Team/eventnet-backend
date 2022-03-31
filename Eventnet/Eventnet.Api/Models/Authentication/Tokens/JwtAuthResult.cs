using System.IdentityModel.Tokens.Jwt;
using Eventnet.Api.Services;

namespace Eventnet.Api.Models.Authentication.Tokens;

public record JwtAuthResult(JwtSecurityToken AccessToken, RefreshToken RefreshToken);