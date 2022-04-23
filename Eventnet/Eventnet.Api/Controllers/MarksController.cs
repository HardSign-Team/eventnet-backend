using Eventnet.Api.Helpers;
using Eventnet.Api.Models.Marks;
using Eventnet.Api.Services;
using Eventnet.DataAccess;
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
    public async Task<IActionResult> AddLike(Guid eventId)
    {
        if (eventId == Guid.Empty)
            return NotFound();
        var user = await currentUserService.GetCurrentUser() ?? throw new Exception();
        var mark = await dbContext.Marks.Of(user).For(eventId).FirstOrDefaultAsync();
        if (mark is null)
        {
            var eventEntity = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            if (eventEntity is null)
                return NotFound();
            mark = eventEntity.Like(user);
            await dbContext.Marks.AddAsync(mark);
        }
        else
        {
            mark.Like();
        }

        await dbContext.SaveChangesAsync();

        return await CountMarks(eventId);
    }

    [Authorize]
    [HttpPost("likes/remove/{eventId:guid}")]
    public async Task<IActionResult> RemoveLike(Guid eventId)
    {
        if (eventId == Guid.Empty)
            return NotFound();

        var eventExists = await dbContext.Events.AnyAsync(x => x.Id == eventId);
        if (!eventExists)
            return NotFound();

        var user = await currentUserService.GetCurrentUser() ?? throw new Exception();
        var likes = dbContext.Marks.Likes().Of(user).For(eventId);
        dbContext.Marks.RemoveRange(likes);
        await dbContext.SaveChangesAsync();

        return await CountMarks(eventId);
    }

    [Authorize]
    [HttpPost("dislikes/add/{eventId:guid}")]
    public async Task<IActionResult> AddDislike(Guid eventId)
    {
        if (eventId == Guid.Empty)
            return NotFound();

        var user = await currentUserService.GetCurrentUser() ?? throw new Exception();
        var mark = await dbContext.Marks.Of(user).For(eventId).FirstOrDefaultAsync();
        if (mark is null)
        {
            var eventEntity = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            if (eventEntity is null)
                return NotFound();
            mark = eventEntity.Dislike(user);
            await dbContext.Marks.AddAsync(mark);
        }
        else
        {
            mark.Dislike();
        }

        await dbContext.SaveChangesAsync();

        return await CountMarks(eventId);
    }

    [Authorize]
    [HttpPost("dislikes/remove/{eventId:guid}")]
    public async Task<IActionResult> RemoveDislike(Guid eventId)
    {
        throw new NotImplementedException();
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