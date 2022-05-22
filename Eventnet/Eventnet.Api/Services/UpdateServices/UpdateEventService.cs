using System.Text.Json;
using Eventnet.Api.Services.SaveServices;
using Eventnet.Domain.Events;
using Eventnet.Infrastructure;
using Eventnet.Infrastructure.UpdateServices;

namespace Eventnet.Api.Services.UpdateServices;

public class EventUpdateService : IEventUpdateService
{
    private readonly IPublishEventService publishEventService;
    private const string RoutingKey = "update";

    public EventUpdateService(IPublishEventService publishEventService)
    {
        this.publishEventService = publishEventService;
    }
    
    public async Task SendEventForUpdate(EventInfo eventForUpdate)
    {
        var photos = new List<RabbitMqPhoto>();
        var guids = Array.Empty<Guid>();
        var message = JsonSerializer.Serialize(new RabbitMqUpdateMessage(eventForUpdate.EventId, eventForUpdate, photos, guids));
        await publishEventService.PublishAsync(message, RoutingKey);
    }

    public async Task SendPhotosForUpdate(Guid eventId, IFormFile[] newPhotos, Guid[] idToDelete)
    {
        var photos = await GetRabbitMqPhotosAsync(newPhotos);
        var message = JsonSerializer.Serialize(new RabbitMqUpdateMessage(eventId, null, photos, idToDelete));
        await publishEventService.PublishAsync(message, RoutingKey);
    }
    
    private static async Task<List<RabbitMqPhoto>> GetRabbitMqPhotosAsync(IFormFile[] photos)
    {
        var rabbitMqPhoto = new List<RabbitMqPhoto>();
        foreach (var photo in photos)
        {
            await using var memoryStream = new MemoryStream();
            await photo.CopyToAsync(memoryStream);
            rabbitMqPhoto.Add(new RabbitMqPhoto(memoryStream.ToArray(), photo.ContentType));
        }

        return rabbitMqPhoto;
    }
}