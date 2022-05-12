using AutoMapper;
using Eventnet.Api.Helpers;
using Eventnet.Api.Models;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
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

    public UserController(ApplicationDbContext dbContext, 
        IMapper mapper, UserManager<UserEntity> userManager)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.userManager = userManager;
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
}