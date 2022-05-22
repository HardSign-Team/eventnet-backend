using System.Text.Json;
using Eventnet.Domain.Events;
using Eventnet.Infrastructure;
using Eventnet.Infrastructure.UpdateServices;

namespace Eventnet.Api.Services.UpdateServices;

public class EventUpdateService : IEventUpdateService
{
    private readonly IPublishEventUpdateService publishEventUpdateService;

    public EventUpdateService(IPublishEventUpdateService publishEventUpdateService)
    {
        this.publishEventUpdateService = publishEventUpdateService;
    }
    
    public async Task SendEventForUpdate(EventInfo eventForUpdate)
    {
        var photos = new List<RabbitMqPhoto>();
        var guids = Array.Empty<Guid>();
        var rabbitMqUpdateMessage = new RabbitMqUpdateMessage(eventForUpdate.EventId, eventForUpdate, photos, guids);
        var message = JsonSerializer.Serialize(rabbitMqUpdateMessage);
        await publishEventUpdateService.PublishAsync(message);
    }

    public async Task SendPhotosForUpdate(Guid eventId, IFormFile[] newPhotos, Guid[] idToDelete)
    {
        var photos = await GetRabbitMqPhotosAsync(newPhotos);
        var rabbitMqUpdateMessage = new RabbitMqUpdateMessage(eventId, null, photos, idToDelete);
        var message = JsonSerializer.Serialize(rabbitMqUpdateMessage);
        await publishEventUpdateService.PublishAsync(message);
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