using System.Text;
using Eventnet.Api.Models.Events;
using Eventnet.Api.Models.Photos;
using Eventnet.Api.Services.Photo;
using Microsoft.AspNetCore.Mvc;

namespace Eventnet.Api.Controllers;

[Route("api/photos")]
public class PhotoController : Controller
{
    private readonly IPhotoService photoService;

    public PhotoController(IPhotoService photoService)
    {
        this.photoService = photoService;
    }

    [HttpPost("title")]
    [Produces(typeof(List<PhotoViewModel>))]
    public async Task<IActionResult> GetTitlePhotos([FromBody] EventIdsListModel? model)
    {
        if (model is null)
            return BadRequest();

        var basePath = GetBaseUrl();
        var viewModels = await photoService.GetTitlePhotos(basePath, model.Ids);

        return Ok(viewModels);
    }

    [HttpGet("{eventId:guid}")]
    [Produces(typeof(List<string>))]
    public async Task<IActionResult> GetPhoto(Guid eventId)
    {
        if (eventId == Guid.Empty)
            return BadRequest();

        var basePath = GetBaseUrl();
        var urls = await photoService.GetPhotoUrls(basePath, eventId);

        return Ok(urls);
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