using Eventnet.Domain;
using Eventnet.Domain.Events;
using Eventnet.Infrastructure.PhotoServices;
using Eventnet.Infrastructure.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Eventnet.Infrastructure;

public class RabbitMqMessageHandler : IRabbitMqMessageHandler
{
    private readonly EventSaveHandler eventSaveHandler;
    private readonly IEventCreationValidator validator;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly RabbitMqMessageHandlerHelper rabbitMqMessageHandlerHelper;

    public RabbitMqMessageHandler(
        EventSaveHandler eventSaveHandler,
        IEventCreationValidator validator,
        IServiceScopeFactory serviceScopeFactory)
    {
        rabbitMqMessageHandlerHelper = new RabbitMqMessageHandlerHelper();
        this.eventSaveHandler = eventSaveHandler;
        this.validator = validator;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task HandleAsync(RabbitMqSaveMessage rabbitMqSaveMessage)
    {
        var (eventForSave, binaryPhotos) = rabbitMqSaveMessage;
        var errorMessage = string.Empty;
        EventSaveStatus status;
        try
        {
            var photos = rabbitMqMessageHandlerHelper.GetPhotos(binaryPhotos);
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
        var eventInfoService = scope.ServiceProvider.GetRequiredService<IEventSaveToDbService>();
        await eventInfoService.SaveEventAsync(info);

        var photosService = scope.ServiceProvider.GetRequiredService<IPhotosDbService>();
        await photosService.SavePhotosAsync(photos, info.EventId);
    }
}