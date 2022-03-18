﻿using System.Text.Json;
using Eventnet.Models;
using Eventnet.Services.ImageServices;

namespace Eventnet.Services;

public class EventSaveService : IEventSaveService
{
    private readonly IPublishEventService publishEventService;
    private readonly IPhotosToTempSaveService photosToTempSaveService;
    private readonly Handler handler;

    public EventSaveService(IPublishEventService publishEventService, 
        IPhotosToTempSaveService photosToTempSaveService, Handler handler)
    {
        this.publishEventService = publishEventService;
        this.photosToTempSaveService = photosToTempSaveService;
        this.handler = handler;
    }

    public async void Save(Event savedEvent, IFormFile[] photos)
    {
        var path = photosToTempSaveService.SaveToTemp(savedEvent.Id, photos);
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
}