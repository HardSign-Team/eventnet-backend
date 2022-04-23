using System.Text;
using System.Text.Json;
using AutoMapper;
using Eventnet.Api.Helpers;
using Eventnet.Api.Models.Events;
using Eventnet.Api.Models.Filtering;
using Eventnet.Api.Models.Marks;
using Eventnet.Api.Models.Tags;
using Eventnet.Api.Services.Filters;
using Eventnet.DataAccess;
using Eventnet.Domain.Events;
using Eventnet.Domain.Selectors;
using Eventnet.Infrastructure;
using Eventnet.Services.SaveServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Eventnet.Controllers;

[Route("api/events")]
public class EventController : Controller
{
    public const int MaxPageSize = 20;
    public const int DefaultPageSize = 10;
    private const int Timeout = 20;
    private static readonly string[] SupportedContentTypes = { "image/bmp", "image/png", "image/jpeg" };
    private readonly IEventFilterMapper filterMapper;
    private readonly ApplicationDbContext dbContext;
    private readonly IEventSaveService eventSaveService;
    private readonly RabbitMqConfig rabbitMqConfig;
    private readonly LinkGenerator linkGenerator;
    private readonly IMapper mapper;

    public EventController(
        IEventFilterMapper filterMapper,
        ApplicationDbContext dbContext,
        IMapper mapper,
        LinkGenerator linkGenerator,
        IEventSaveService eventSaveService,
        RabbitMqConfig rabbitMqConfig)
    {
        this.filterMapper = filterMapper;
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.linkGenerator = linkGenerator;
        this.eventSaveService = eventSaveService;
        this.rabbitMqConfig = rabbitMqConfig;
    }

    [HttpGet("{eventId:guid}")]
    public async Task<IActionResult> GetEventById(Guid eventId)
    {
        if (Guid.Empty == eventId)
        {
            ModelState.AddModelError(nameof(eventId), $"{nameof(eventId)} should not be empty");
            return UnprocessableEntity(ModelState);
        }

        var entity = await dbContext.Events
            .AsNoTracking()
            .Select(x => new
            {
                x.Id,
                x.OwnerId,
                x.Description,
                x.Location,
                x.StartDate,
                x.EndDate,
                x.Name,
                x.Tags,
                TotalSubscriptions = x.Subscriptions.Count,
                Likes = x.Marks.Count(mark => mark.IsLike),
                Dislikes = x.Marks.Count(mark => !mark.IsLike)
            })
            .FirstOrDefaultAsync(x => x.Id == eventId);
        if (entity is null)
            return NotFound();
        var eventViewModel = new EventViewModel(entity.Id,
            entity.OwnerId,
            entity.Name,
            entity.Description,
            mapper.Map<LocationViewModel>(entity.Location),
            entity.StartDate,
            entity.EndDate,
            entity.Tags.Select(mapper.Map<TagNameModel>).ToArray(),
            entity.TotalSubscriptions,
            new MarksCountViewModel(entity.Likes, entity.Dislikes));
        return Ok(eventViewModel);
    }

    [HttpGet("search/name/{eventName}")]
    public IActionResult GetEventsByName(string? eventName, [FromQuery(Name = "m")] int maxCount = 10)
    {
        eventName = eventName?.Trim();
        switch (eventName)
        {
            case null:
                return BadRequest($"{nameof(eventName)} undefined");
            case "":
                return UnprocessableEntity($"Expected {nameof(eventName)} is non-empty string");
        }

        var selector = new EventsByNameSelector(eventName);
        var result = selector
            .Select(mapper.ProjectTo<EventName>(dbContext.Events).AsNoTracking().AsEnumerable(), maxCount)
            .ToArray();

        return Ok(new EventNameListModel(result.Length, result));
    }

    [HttpGet(Name = nameof(GetEvents))]
    public IActionResult GetEvents(
        [FromQuery(Name = "f")] string? filterModelBase64,
        [FromQuery(Name = "p")] int pageNumber = 1,
        [FromQuery(Name = "ps")] int pageSize = DefaultPageSize)
    {
        if (filterModelBase64 is null)
            return BadRequest($"{nameof(filterModelBase64)} was null");
        var filterModel = ParseEventsFilterModel(filterModelBase64);
        if (filterModel is null)
            return BadRequest("Cannot parse filter model");

        TryValidateModel(filterModel);

        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        pageNumber = NumberHelper.Normalize(pageNumber, 1);
        pageSize = NumberHelper.Normalize(pageSize, 1, MaxPageSize);

        var query = dbContext.Events
            .AsNoTracking()
            .AsEnumerable()
            .Select(x => mapper.Map<Event>(x));
        var filter = filterMapper.Map(filterModel);
        var filteredEvents = filter.Filter(query);

        var events = new PagedList<Event>(filteredEvents, pageNumber, pageSize);
        var paginationHeader = events
            .ToPaginationHeader((p, ps) => GenerateEventsPageLink(filterModelBase64, p, ps));

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader));

        return Ok(mapper.Map<IEnumerable<EventLocationModel>>(events));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateEvent([FromForm] CreateEventModel createModel)
    {
        var photos = createModel.Photos;

        if (!IsContentTypesSupported(photos))
            return BadRequest("Not supported ContentType");

        if (!IsPhotosSizeLessThanRecommended(photos))
        {
            var recommendedSizeInMb = rabbitMqConfig.RecommendedMessageSizeInBytes / 1024 / 1024;
            return BadRequest($"Too large images. Recommended size of all images is {recommendedSizeInMb}Mb.");
        }

        var isSaved = await IsEventSaved(createModel.Id);
        if (IsSaveEventBeingHandling(createModel.Id) || isSaved)
            return BadRequest("One event id provided two times");

        var createdEvent = mapper.Map<Event>(createModel);
        await eventSaveService.RequestSave(createdEvent, photos);
        return Accepted();
    }

    [HttpGet("is-created")]
    [Authorize]
    public IActionResult IsCreated(Guid id)
    {
        var (saveStatus, exception) = eventSaveService.GetSaveEventResult(id);
        return saveStatus switch
        {
            EventSaveStatus.Saved => Ok(),
            EventSaveStatus.NotSavedDueToUserError => BadRequest(exception),
            EventSaveStatus.NotSavedDueToServerError => BadRequest(exception),
            EventSaveStatus.InProgress => Accepted(Timeout),
            _ => throw new ArgumentOutOfRangeException($"Unknown SaveState {saveStatus}")
        };
    }

    // TODO use format https://datatracker.ietf.org/doc/html/rfc6902
    [HttpPatch("{eventId:guid}")]
    public IActionResult UpdateEvent(Guid eventId, [FromBody] UpdateEventModel updateModel) =>
        throw new NotImplementedException();

    [HttpDelete("{eventId:guid}")]
    public async Task<IActionResult> DeleteEvent(Guid eventId)
    {
        var eventEntity = await dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        if (eventEntity is null)
            return NotFound();

        dbContext.Events.Remove(eventEntity);
        await dbContext.SaveChangesAsync();

        return Ok(new { eventId });
    }

    [HttpGet("request-event-creation")]
    [Authorize]
    public IActionResult RequestEventCreation()
    {
        var id = Guid.NewGuid();
        return Ok(id);
    }

    private static bool IsContentTypesSupported(IFormFile[] files)
    {
        var contentTypes = files.Select(photo => photo.ContentType);
        return contentTypes.All(SupportedContentTypes.Contains);
    }

    private bool IsPhotosSizeLessThanRecommended(IFormFile[] photos)
    {
        var photosSize = photos.Sum(photo => photo.Length);
        return photosSize >= rabbitMqConfig.RecommendedMessageSizeInBytes;
    }

    private bool IsSaveEventBeingHandling(Guid id) => eventSaveService.IsHandling(id);

    private async Task<bool> IsEventSaved(Guid id)
    {
        var eventInDb = await dbContext.Events.FindAsync(id);
        return eventInDb is not null;
    }

    private static EventsFilterModel? ParseEventsFilterModel(string base64Model)
    {
        try
        {
            var bytes = Convert.FromBase64String(base64Model);
            var json = Encoding.Default.GetString(bytes);
            return JsonSerializer.Deserialize<EventsFilterModel>(json);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private string? GenerateEventsPageLink(string filterModelBase64, int pageNumber, int pageSize)
    {
        var values = new { f = filterModelBase64, p = pageNumber, ps = pageSize };
        return linkGenerator.GetUriByRouteValues(HttpContext, nameof(GetEvents), values);
    }
}