using System.Text.Json;
using Eventnet.Domain.Events;
using Eventnet.Infrastructure;

namespace Eventnet.Api.Services.SaveServices;

public class EventSaveService : IEventSaveService
{
    private readonly EventSaveHandler eventSaveHandler;
    private readonly IPublishEventSaveService publishEventSaveService;

    public EventSaveService(IPublishEventSaveService publishEventSaveService, EventSaveHandler eventSaveHandler)
    {
        this.publishEventSaveService = publishEventSaveService;
        this.eventSaveHandler = eventSaveHandler;
    }

    public async Task RequestSave(EventInfo eventForSave, IFormFile[] photos)
    {
        var saveEventResult = new SaveEventResult(EventSaveStatus.InProgress, string.Empty);
        eventSaveHandler.Update(eventForSave.EventId, saveEventResult);
        try
        {
            var rabbitMqPhotos = await GetRabbitMqPhotosAsync(photos);
            var message = JsonSerializer.Serialize(new RabbitMqSaveMessage(eventForSave, rabbitMqPhotos));
            await publishEventSaveService.PublishAsync(message);
        }
        catch (Exception e)
        {
            saveEventResult = new SaveEventResult(EventSaveStatus.NotSavedDueToServerError, string.Empty);
            eventSaveHandler.Update(eventForSave.EventId, saveEventResult);
            Console.WriteLine(e); // TODO: add logger
            throw;
        }
    }

    public bool IsHandling(Guid id) => eventSaveHandler.IsHandling(id);

    public SaveEventResult GetSaveEventResult(Guid id)
    {
        if (!eventSaveHandler.TryGetValue(id, out var saveEventResult))
            return new SaveEventResult(EventSaveStatus.NotSavedDueToUserError, "No such Id");
        return saveEventResult;
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