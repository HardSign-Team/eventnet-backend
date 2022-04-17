using Eventnet.Infrastructure;

namespace Eventnet.Services.SaveServices;

public interface IConsumeEventService : IDisposable
{
    void ConsumeAndHandle(Action<RabbitMqMessage> handle);
}