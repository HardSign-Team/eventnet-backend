using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventnet.Api.Controllers;

[Route("api/subscriptions")]
public class SubscriptionsController : Controller
{
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
        throw new NotImplementedException();
    }
}