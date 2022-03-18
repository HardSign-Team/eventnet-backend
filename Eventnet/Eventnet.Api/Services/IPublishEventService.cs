namespace Eventnet.Services;

public interface IPublishEventService
{
    Task SendAsync(string message);
}