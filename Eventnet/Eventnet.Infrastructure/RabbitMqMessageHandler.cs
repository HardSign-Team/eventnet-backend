﻿using System.Drawing;
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
        var exception = string.Empty;
        EventSaveStatus status;
        try
        {
            var photos = GetPhotos(binaryPhotos);
            var result = validator.Validate(photos, eventForSave);
            exception = result.Exception;
            if (result.IsOk)
            {
                saveToDbService.SaveEvent(eventForSave);
                saveToDbService.SavePhotos(photos, id);
            }

            status = result.IsOk ? EventSaveStatus.Saved : EventSaveStatus.NotSavedDueToUserError;
        }
        catch (Exception e)
        {
            exception = "Something went wrong on server. Please try again later" + "\n" + exception;
            status = EventSaveStatus.NotSavedDueToServerError;
            Console.WriteLine(e);
        }
        handler.Update(id, new SaveEventResult(status, exception));
    }
    
    private List<Image> GetPhotos(List<byte[]> binaryPhotos) 
        => binaryPhotos
            .Select(binaryPhoto => new MemoryStream(binaryPhoto))
            .Select(Image.FromStream)
            .ToList();
}