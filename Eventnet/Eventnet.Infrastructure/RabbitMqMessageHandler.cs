using Eventnet.Domain;
using Eventnet.Domain.Events;
using Eventnet.Infrastructure.Validators;

namespace Eventnet.Infrastructure;

public class RabbitMqMessageHandler : IRabbitMqMessageHandler
{
    private readonly EventSaveHandler eventSaveHandler;
    private readonly ISaveToDbService saveToDbService;
    private readonly IEventCreationValidator validator;

    public RabbitMqMessageHandler(
        EventSaveHandler eventSaveHandler,
        IEventCreationValidator validator,
        ISaveToDbService saveToDbService)
    {
        this.eventSaveHandler = eventSaveHandler;
        this.validator = validator;
        this.saveToDbService = saveToDbService;
    }

    public void Handle(RabbitMqMessage rabbitMqMessage)
    {
        var (eventForSave, binaryPhotos) = rabbitMqMessage;
        var errorMessage = string.Empty;
        EventSaveStatus status;
        try
        {
            var photos = GetPhotos(binaryPhotos);
            (var isOk, errorMessage) = validator.Validate(photos, eventForSave);
            if (isOk)
            {
                SaveEvent(eventForSave, photos);
            }

            status = isOk ? EventSaveStatus.Saved : EventSaveStatus.NotSavedDueToUserError;
        }
        catch (Exception e)
        {
            errorMessage = "Something went wrong on server. Please try again later" + "\n" + errorMessage;
            status = EventSaveStatus.NotSavedDueToServerError;
            Console.WriteLine(e); // TODO: add logger
        }

        eventSaveHandler.Update(eventForSave.EventId, new SaveEventResult(status, errorMessage));
    }

    private void SaveEvent(EventInfo info, List<Photo> photos)
    {
        saveToDbService.SaveEventAsync(info);
        saveToDbService.SavePhotosAsync(photos, info.EventId);
    }

    private static List<Photo> GetPhotos(IEnumerable<RabbitMqPhoto> rabbitMqPhotos) =>
        rabbitMqPhotos
            .Select(rabbitMqPhoto => new Photo(rabbitMqPhoto.PhotoInBytes, rabbitMqPhoto.ContentType))
            .ToList();
}