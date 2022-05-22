namespace Eventnet.Api.Services.UpdateServices;

public interface IPublishEventUpdateService
{
    Task PublishAsync(string message);
}