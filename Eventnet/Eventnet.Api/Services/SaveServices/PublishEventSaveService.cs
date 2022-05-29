using Eventnet.Api.Config;

namespace Eventnet.Api.Services.SaveServices;

public class PublishEventSaveService : EventPublisher, IPublishEventSaveService
{
    public PublishEventSaveService(RabbitMqConfig config) : base(config.QueueEventSave, config.HostName, config.Port)
    {
    }
}