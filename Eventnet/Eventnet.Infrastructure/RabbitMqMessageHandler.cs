using Eventnet.Domain;
using Eventnet.Infrastructure.Validators;

namespace Eventnet.Infrastructure;

public class RabbitMqMessageHandler : IRabbitMqMessageHandler
{
    private readonly Handler handler;
    private readonly ISaveToDbService saveToDbService;
    private readonly IEventCreationValidator validator;

    public RabbitMqMessageHandler(Handler handler, IEventCreationValidator validator,
        ISaveToDbService saveToDbService)
    {
        this.handler = handler;
        this.validator = validator;
        this.saveToDbService = saveToDbService;
    }

    public void Handle(RabbitMqMessage rabbitMqMessage)
    {
        var (eventForSave, binaryPhotos) = rabbitMqMessage;
        var id = eventForSave.Id;
        var errorMessage = string.Empty;
        EventSaveStatus status;
        try
        {
            var photos = GetPhotos(binaryPhotos);
            var result = validator.Validate(photos, eventForSave);
            errorMessage = result.ErrorMessage;
            if (result.IsOk)
            {
                saveToDbService.SaveEvent(eventForSave);
                saveToDbService.SavePhotos(photos, id);
            }

            status = result.IsOk ? EventSaveStatus.Saved : EventSaveStatus.NotSavedDueToUserError;
        }
        catch (Exception e)
        {
            errorMessage = "Something went wrong on server. Please try again later" + "\n" + errorMessage;
            status = EventSaveStatus.NotSavedDueToServerError;
            Console.WriteLine(e);
        }
        handler.Update(id, new SaveEventResult(status, errorMessage));
    }

    private List<Photo> GetPhotos(List<RabbitMqPhoto> rabbitMqPhotos) => 
        rabbitMqPhotos
            .Select(rabbitMqPhoto => new Photo(rabbitMqPhoto.PhotoInBytes, rabbitMqPhoto.ContentType))
            .ToList();
}