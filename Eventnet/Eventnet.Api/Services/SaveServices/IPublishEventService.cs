namespace Eventnet.Services.SaveServices;

public interface IPublishEventService : IDisposable
{
    Task SendAsync(string message);
}