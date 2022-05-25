namespace Eventnet.Api.Services.UpdateServices;

public interface IPublishUpdateEventService
{
    Task PublishAsync(string message);
}