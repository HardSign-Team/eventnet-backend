using Eventnet.Infrastructure.ImageServices;

namespace Eventnet.Infrastructure;

public class RabbitMqMessageHandler : IRabbitMqMessageHandler
{
    private readonly Handler handler;
    private readonly ILoadFromTempService loadFromTempService;
    private readonly ISaveToDbService saveToDbService;
    private readonly IDeleteFromTempFolderService deleteFromTempFolderService;
    private readonly IImageValidator validator;

    public RabbitMqMessageHandler(Handler handler, ILoadFromTempService loadFromTempService, IImageValidator validator,
        ISaveToDbService saveToDbService, IDeleteFromTempFolderService deleteFromTempFolderService)
    {
        this.handler = handler;
        this.loadFromTempService = loadFromTempService;
        this.validator = validator;
        this.saveToDbService = saveToDbService;
        this.deleteFromTempFolderService = deleteFromTempFolderService;
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
            deleteFromTempFolderService.Delete(rabbitMqMessage.PathToPhotos);
        }
        catch (Exception e)
        {
            exception = "Something went wrong on server. Please try again later";
            Console.WriteLine(e);
        }

        handler.Update(id, new SaveEventResult(exception.Length == 0, exception));
    }
}