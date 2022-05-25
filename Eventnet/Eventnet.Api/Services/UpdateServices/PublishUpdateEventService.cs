using Eventnet.Api.Config;
using Eventnet.Api.Services.SaveServices;

namespace Eventnet.Api.Services.UpdateServices;

public class PublishUpdateEventService : EventPublisher, IPublishUpdateEventService
{
    public PublishUpdateEventService(RabbitMqConfig config) : base(config.QueueEventUpdate, config.HostName, config.Port)
    {
        
    }
}