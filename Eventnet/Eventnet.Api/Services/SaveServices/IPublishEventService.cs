namespace Eventnet.Services.SaveServices;

public interface IPublishEventService : IDisposable
{
    Task PublishAsync(string message);
}