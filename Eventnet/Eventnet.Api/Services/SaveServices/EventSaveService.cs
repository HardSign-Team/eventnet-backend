using System.Text.Json;
using Eventnet.Infrastructure;
using Eventnet.Models;

namespace Eventnet.Services.SaveServices;

public class EventSaveService : IEventSaveService
{
    private readonly Handler handler;
    private readonly IPublishEventService publishEventService;

    public EventSaveService(IPublishEventService publishEventService, Handler handler)
    {
        this.publishEventService = publishEventService;
        this.handler = handler;
    }

    public async Task SaveAsync(Event savedEvent, IFormFile[] photos)
    {
        var streams = await GetStreamsAsync(photos);
        var message = JsonSerializer.Serialize(new RabbitMqMessage(savedEvent, streams));
        handler.Update(savedEvent.Id, new SaveEventResult(false, string.Empty));
        await publishEventService.SendAsync(message);
    }

    public bool IsEventSaved(Guid id, out string exceptionValue)
    {
        exceptionValue = "";
        if (!handler.TryGetValue(id, out var saveEventResult))
            return false;
        exceptionValue = saveEventResult.ExceptionInformation;
        return saveEventResult.IsSaved;
    }

    private async Task<List<byte[]>> GetStreamsAsync(IFormFile[] photos)
    {
        var bytes = new List<byte[]>();
        foreach (var photo in photos)
        {
            await using var memoryStream = new MemoryStream();
            await photo.CopyToAsync(memoryStream);
            bytes.Add(memoryStream.ToArray());
        }

        return bytes;
    }
}