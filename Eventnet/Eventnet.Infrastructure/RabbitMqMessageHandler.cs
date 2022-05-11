using Eventnet.Domain;
using Eventnet.Domain.Events;
using Eventnet.Infrastructure.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Eventnet.Infrastructure;

public class RabbitMqMessageHandler : IRabbitMqMessageHandler
{
    private readonly EventSaveHandler eventSaveHandler;
    private readonly IEventCreationValidator validator;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public RabbitMqMessageHandler(
        EventSaveHandler eventSaveHandler,
        IEventCreationValidator validator,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.eventSaveHandler = eventSaveHandler;
        this.validator = validator;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(RabbitMqMessage rabbitMqMessage)
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
                await SaveEventAsync(eventForSave, photos);
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

    private async Task SaveEventAsync(EventInfo info, List<Photo> photos)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ISaveToDbService>();
        await service.SaveEventAsync(info);
        await service.SavePhotosAsync(photos, info.EventId);
    }

    private static List<Photo> GetPhotos(IEnumerable<RabbitMqPhoto> rabbitMqPhotos) =>
        rabbitMqPhotos
            .Select(rabbitMqPhoto => new Photo(rabbitMqPhoto.PhotoInBytes, rabbitMqPhoto.ContentType))
            .ToList();
}