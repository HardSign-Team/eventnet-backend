using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.Helpers;
using Eventnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Controllers;

[Route("api/users")]
public class UserController : Controller
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public UserController(ApplicationDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    [HttpGet("searchByPrefix/{prefix:alpha:required}")]
    public async Task<IActionResult> GetUsers(string prefix, [FromQuery] int maxUsers = 100)
    {
        maxUsers = NumberHelper.Normalize(maxUsers, 10, 100);

        var query = dbContext.Users
            .Where(x => x.UserName.StartsWith(prefix))
            .OrderBy(x => x.UserName)
            .Take(maxUsers);
        var result = await mapper.ProjectTo<UserNameModel>(query).ToArrayAsync();

        return Ok(new UserNameListModel(result.Length, result));
    }
}