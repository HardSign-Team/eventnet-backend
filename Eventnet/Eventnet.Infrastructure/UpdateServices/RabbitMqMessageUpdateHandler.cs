using Eventnet.Domain.Events;
using Eventnet.Infrastructure.PhotoServices;
using Eventnet.Infrastructure.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Eventnet.Infrastructure.UpdateServices;

public class RabbitMqMessageUpdateHandler : IRabbitMqMessageUpdateHandler
{
    private readonly IEventValidator eventValidator;
    private readonly IPhotoValidator photoValidator;
    private readonly RabbitMqMessageHandlerHelper rabbitMqMessageHandlerHelper;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public RabbitMqMessageUpdateHandler(
        IPhotoValidator photoValidator,
        IEventValidator eventValidator,
        IServiceScopeFactory serviceScopeFactory)
    {
        rabbitMqMessageHandlerHelper = new RabbitMqMessageHandlerHelper();
        this.photoValidator = photoValidator;
        this.eventValidator = eventValidator;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task UpdateAsync(RabbitMqUpdateMessage rabbitMqMessageSave)
    {
        var (eventId, info, binaryPhotos, guidsToDelete) = rabbitMqMessageSave;
        if (binaryPhotos.Count > 0)
            await AddPhotosHandle(binaryPhotos, eventId);

        if (info is not null)
            await UpdateEvent(info);

        if (guidsToDelete.Length > 0)
            await DeletePhotos(guidsToDelete);
    }

    private async Task AddPhotosHandle(List<RabbitMqPhoto> binaryPhotos, Guid eventId)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPhotosDbService>();
        var photos = rabbitMqMessageHandlerHelper.GetPhotos(binaryPhotos);
        var result = photoValidator.Validate(photos);
        if (result.IsOk)
            await service.SavePhotosAsync(photos, eventId);
    }

    private async Task UpdateEvent(EventInfo eventForUpdate)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IEventSaveToDbService>();
        var result = eventValidator.Validate(eventForUpdate);
        if (result.IsOk)
            await service.UpdateEventAsync(eventForUpdate);
    }

    private async Task DeletePhotos(Guid[] guidsToDelete)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IPhotosDbService>();
        await service.DeletePhotosAsync(guidsToDelete);
    }
}