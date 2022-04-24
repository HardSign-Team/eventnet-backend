using Eventnet.Api.Models.Marks;
using Eventnet.Api.Services;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.DataAccess.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Api.Controllers;

[Route("api/marks")]
public class MarksController : Controller
{
    private readonly ApplicationDbContext dbContext;
    private readonly CurrentUserService currentUserService;

    public MarksController(ApplicationDbContext dbContext, CurrentUserService currentUserService)
    {
        this.dbContext = dbContext;
        this.currentUserService = currentUserService;
    }

    [Authorize]
    [HttpPost("likes/add/{eventId:guid}")]
    [Produces(typeof(MarksCountViewModel))]
    public async Task<IActionResult> AddLike(Guid eventId)
    {
        return await HandleAddMark(eventId,
            (eventEntity, user) => eventEntity.Like(user),
            mark => mark.Like());
    }

    [Authorize]
    [HttpPost("likes/remove/{eventId:guid}")]
    [Produces(typeof(MarksCountViewModel))]
    public async Task<IActionResult> RemoveLike(Guid eventId)
    {
        return await HandleRemoveMarks(eventId, context => context.Marks.Likes());
    }

    [Authorize]
    [HttpPost("dislikes/add/{eventId:guid}")]
    [Produces(typeof(MarksCountViewModel))]
    public async Task<IActionResult> AddDislike(Guid eventId)
    {
        return await HandleAddMark(eventId,
            (eventEntity, user) => eventEntity.Dislike(user),
            mark => mark.Dislike());
    }

    [Authorize]
    [HttpPost("dislikes/remove/{eventId:guid}")]
    [Produces(typeof(MarksCountViewModel))]
    public async Task<IActionResult> RemoveDislike(Guid eventId)
    {
        return await HandleRemoveMarks(eventId, context => context.Marks.Dislikes());
    }

    private async Task<IActionResult> HandleRemoveMarks(
        Guid eventId,
        Func<ApplicationDbContext, IQueryable<MarkEntity>> marks)
    {
        if (eventId == Guid.Empty)
            return NotFound();

        var eventExists = await dbContext.Events.AnyAsync(x => x.Id == eventId);
        if (!eventExists)
            return NotFound();

        var user = await currentUserService.GetCurrentUser();
        if (user is null)
            return Unauthorized();
        
        var filtered = marks(dbContext).Of(user).For(eventId);
        dbContext.Marks.RemoveRange(filtered);
        await dbContext.SaveChangesAsync();

        return await CountMarks(eventId);
    }

    private async Task<IActionResult> HandleAddMark(
        Guid eventId,
        Func<EventEntity, UserEntity, MarkEntity> createMark,
        Action<MarkEntity> changeMark)
    {
        if (eventId == Guid.Empty)
            return NotFound();
        
        var user = await currentUserService.GetCurrentUser();
        if (user is null) 
            return Unauthorized();
        
        var mark = await dbContext.Marks.Of(user).For(eventId).FirstOrDefaultAsync();
        if (mark is null)
        {
            var eventEntity = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            if (eventEntity is null)
                return NotFound();
            mark = createMark(eventEntity, user);
            await dbContext.Marks.AddAsync(mark);
        }
        else
        {
            changeMark(mark);
        }

        await dbContext.SaveChangesAsync();

        return await CountMarks(eventId);
    }

    private async Task<IActionResult> CountMarks(Guid eventId)
    {
        var viewModel = await dbContext.Marks
            .For(eventId)
            .GroupBy(x => x.EventId)
            .Select(x => new MarksCountViewModel(x.Count(y => y.IsLike),
                x.Count(y => !y.IsLike)))
            .FirstOrDefaultAsync() ?? new MarksCountViewModel(0, 0);
        return Ok(viewModel);
    }
}