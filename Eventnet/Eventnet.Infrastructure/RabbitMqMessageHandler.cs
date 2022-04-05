using System.Drawing;
using Eventnet.Infrastructure.PhotoServices;

namespace Eventnet.Infrastructure;

public class RabbitMqMessageHandler : IRabbitMqMessageHandler
{
    private readonly Handler handler;
    private readonly ISaveToDbService saveToDbService;
    private readonly IPhotoValidator validator;

    public RabbitMqMessageHandler(Handler handler, IPhotoValidator validator,
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
        var exception = "";
        try
        {
            var photos = GetPhotos(binaryPhotos);
            if (validator.Validate(photos, out exception))
            {
                saveToDbService.SaveEvent(eventForSave);
                saveToDbService.SavePhotos(photos, id);
            }
        }
        catch (Exception e)
        {
            exception = "Something went wrong on server. Please try again later";
            Console.WriteLine(e);
        }

        handler.Update(id, new SaveEventResult(exception.Length == 0, exception));
    }
    
    private List<Image> GetPhotos(List<byte[]> binaryPhotos) 
        => binaryPhotos
            .Select(binaryPhoto => new MemoryStream(binaryPhoto))
            .Select(Image.FromStream)
            .ToList();
}