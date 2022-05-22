namespace Eventnet.Api.Services.SaveServices;

public interface IPublishEventSaveService
{
    Task PublishAsync(string message);
}