using Eventnet.Services.ImageServices;

namespace Eventnet.Services;

public class RabbitMqMessageHandler : IRabbitMqMessageHandler
{
    private readonly Handler handler;
    private readonly ILoadFromTempService loadFromTempService;
    private readonly ISaveToDbService saveToDbService;
    private readonly IImageValidator validator;

    public RabbitMqMessageHandler(Handler handler, ILoadFromTempService loadFromTempService, IImageValidator validator,
        ISaveToDbService saveToDbService)
    {
        this.handler = handler;
        this.loadFromTempService = loadFromTempService;
        this.validator = validator;
        this.saveToDbService = saveToDbService;
    }

    public void Handle(RabbitMqMessage rabbitMqMessage)
    {
        var id = rabbitMqMessage.Event.Id;
        var exception = "";
        try
        {
            var photos = loadFromTempService.LoadImages(rabbitMqMessage.PathToPhotos);
            if (validator.Validate(photos, out exception))
            {
                saveToDbService.SaveImages(photos, id);
                saveToDbService.SaveEvent(rabbitMqMessage);
            }
        }
        catch (Exception)
        {
            exception = "Something went wrong on server. Please try again later";
        }

        handler.Update(id, new SaveEventResult(exception.Length == 0, exception));
    }
}