using System.Text;
using AutoMapper;
using Eventnet.Api.Helpers;
using Eventnet.Api.Models;
using Eventnet.Api.Services;
using Eventnet.Api.Services.UserAvatars;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain;
using Eventnet.Infrastructure.PhotoServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Api.Controllers;

[Route("api/users")]
public class UserController : Controller
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly UserManager<UserEntity> userManager;
    private readonly CurrentUserService currentUserService;
    private readonly IUserAvatarsService userAvatarsService;
    private static readonly string[] SupportedContentTypes = { "image/bmp", "image/png", "image/jpeg" };

    public UserController(ApplicationDbContext dbContext, 
        IMapper mapper, UserManager<UserEntity> userManager, 
        CurrentUserService currentUserService,
        IUserAvatarsService userAvatarsService)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.userManager = userManager;
        this.currentUserService = currentUserService;
        this.userAvatarsService = userAvatarsService;
    }

    [Authorize]
    [HttpPut("{userId:guid}")]
    [Produces(typeof(UserViewModel))]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid userId, [FromBody] UpdateUserForm updateUserForm)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
        
        if (user is null)
            return NotFound();

        dbContext.Attach(user);

        mapper.Map(updateUserForm, user);
        
        await dbContext.SaveChangesAsync();

        return Ok(mapper.Map<UserViewModel>(user));
    }

    [Authorize]
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] FileForm form)
    {
        var user = await currentUserService.GetCurrentUserAsync();
        
        if (user is null)
            return NotFound();
        
        if(SupportedContentTypes.All(x => x != form.Avatar.ContentType))
            return BadRequest("Not supported ContentType");

        var avatarName = await userAvatarsService.UploadAvatarAsync(user, form.Avatar);
        
        return Ok($"{GetBaseUrl()}{avatarName}");
    }

    [HttpGet("search/prefix/{prefix:alpha:required}")]
    [Produces(typeof(List<UserNameListViewModel>))]
    public async Task<IActionResult> GetUsers(string prefix, [FromQuery] int maxUsers = 100)
    {
        maxUsers = NumberHelper.Normalize(maxUsers, 10, 100);

        var query = dbContext.Users
            .Where(x => x.UserName.StartsWith(prefix))
            .OrderBy(x => x.UserName)
            .Take(maxUsers);
        var result = await mapper.ProjectTo<UserNameModel>(query).ToArrayAsync();

        return Ok(new UserNameListViewModel(result.Length, result));
    }
    
    private string GetBaseUrl()
    {
        var sb = new StringBuilder();
        sb.Append($"{Request.Scheme}://{Request.Host.Host}");
        if (Request.Host.Port is { } port)
            sb.Append($":{port}");
        sb.Append('/');
        return sb.ToString();
    }
}

public record FileForm(IFormFile Avatar);