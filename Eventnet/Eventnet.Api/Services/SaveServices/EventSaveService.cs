using System.Text.Json;
using Eventnet.Infrastructure;
using Eventnet.Models;

namespace Eventnet.Services.SaveServices;

public class EventSaveService : IEventSaveService
{
    private readonly EventSaveHandler eventSaveHandler;
    private readonly IPublishEventService publishEventService;

    public EventSaveService(IPublishEventService publishEventService, EventSaveHandler eventSaveHandler)
    {
        this.publishEventService = publishEventService;
        this.eventSaveHandler = eventSaveHandler;
    }

    public async Task RequestSave(Event eventForSave, IFormFile[] photos)
    {
        var saveEventResult = new SaveEventResult(EventSaveStatus.InProgress, string.Empty);
        eventSaveHandler.Update(eventForSave.Id, saveEventResult);
        try
        {
            var rabbitMqPhotos = await GetRabbitMqPhotosAsync(photos);
            var message = JsonSerializer.Serialize(new RabbitMqMessage(eventForSave, rabbitMqPhotos));
            await publishEventService.PublishAsync(message);
        }
        catch (Exception e)
        {
            saveEventResult = new SaveEventResult(EventSaveStatus.NotSavedDueToServerError, string.Empty);
            eventSaveHandler.Update(eventForSave.Id, saveEventResult);
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