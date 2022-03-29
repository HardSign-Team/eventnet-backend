using System.Text.Json;
using Eventnet.Infrastructure;
using Eventnet.Models;

namespace Eventnet.Services.SaveServices;

public class EventSaveService : IEventSaveService
{
    private readonly Handler handler;
    private readonly IPhotosToTempSaveService photosToTempSaveService;
    private readonly IPublishEventService publishEventService;

    public EventSaveService(IPublishEventService publishEventService,
        IPhotosToTempSaveService photosToTempSaveService, Handler handler)
    {
        this.publishEventService = publishEventService;
        this.photosToTempSaveService = photosToTempSaveService;
        this.handler = handler;
    }

    public async Task SaveAsync(Event savedEvent, IFormFile[] photos)
    {
        var streams = GetStreams(photos);
        var path = photosToTempSaveService.SaveToTemp(savedEvent.Id, streams);
        var message = JsonSerializer.Serialize(new RabbitMqMessage(savedEvent, path));
        handler.Update(savedEvent.Id, new SaveEventResult(false, string.Empty));
        await publishEventService.SendAsync(message);
    }

    public bool IsEventSaved(Guid id, out string exceptionValue)
    {
        exceptionValue = "";
        if (!handler.ContainsInformationAbout(id))
            return false;
        var (isSaved, exceptionInformation) = handler.GetValue(id);
        exceptionValue = exceptionInformation;
        return isSaved;
    }
    
    private Stream[] GetStreams(IFormFile[] photos) => photos.Select(photo => photo.OpenReadStream()).ToArray();
}