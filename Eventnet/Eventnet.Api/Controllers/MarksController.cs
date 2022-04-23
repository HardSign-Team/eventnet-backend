using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventnet.Api.Controllers;

[Route("api/marks")]
public class MarksController : Controller
{
    [Authorize]
    [HttpPost("likes/add/{eventId}")]
    public async Task<IActionResult> AddLike(Guid eventId)
    {
        throw new NotImplementedException();
    }
    
    [Authorize]
    [HttpPost("likes/remove/{eventId}")]
    public async Task<IActionResult> RemoveLike(Guid eventId)
    {
        throw new NotImplementedException();
    }
    
    [Authorize]
    [HttpPost("dislikes/add/{eventId}")]
    public async Task<IActionResult> AddDislike(Guid eventId)
    {
        throw new NotImplementedException();
    }
    
    [Authorize]
    [HttpPost("dislikes/remove/{eventId}")]
    public async Task<IActionResult> RemoveDislike(Guid eventId)
    {
        throw new NotImplementedException();
    }
}