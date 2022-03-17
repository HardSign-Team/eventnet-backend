using System.Security.Claims;
using Eventnet.DataAccess;
using Microsoft.AspNetCore.Identity;

namespace Eventnet.Services;

public class CurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly UserManager<UserEntity> userManager;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor,
        UserManager<UserEntity> userManager)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.userManager = userManager;
    }

    public async Task<UserEntity?> GetCurrentUser()
    {
        var userName = GetCurrentUserName();

        if (userName == null)
            return null;

        return await userManager.FindByNameAsync(userName);
    }

    public string? GetCurrentUserName() =>
        httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
}