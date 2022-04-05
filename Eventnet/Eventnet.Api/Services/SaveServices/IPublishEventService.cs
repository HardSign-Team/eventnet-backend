namespace Eventnet.Services.SaveServices;

public interface IPublishEventService
{
    Task SendAsync(string message);
}