using Eventnet.Api.Models.Subscriptions;
using Eventnet.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Api.Controllers;

[Route("api/subscriptions")]
public class SubscriptionsController : Controller
{
    private readonly ApplicationDbContext dbContext;

    public SubscriptionsController(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [Authorize]
    [HttpPost("subscribe/{eventId}")]
    public async Task<IActionResult> Subscribe(Guid eventId)
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpPost("unsubscribe/{eventId}")]
    public async Task<IActionResult> UnSubscribe(Guid eventId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("count/{eventId}")]
    public async Task<IActionResult> GetSubscriptionsCount(Guid eventId)
    {
        if (eventId == Guid.Empty)
            return NotFound();

        var result = await dbContext.Events
            .Select(x => new
            {
                x.Id, 
                SubscriptionsCount = x.Subscriptions.Count()
            })
            .FirstOrDefaultAsync(x => x.Id == eventId);

        if (result is null)
            return NotFound();
        
        return Ok(new SubscriptionsCountViewModel(result.Id, result.SubscriptionsCount));
    }
}