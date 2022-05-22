using Eventnet.Api.Config;
using Eventnet.Api.Services.SaveServices;

namespace Eventnet.Api.Services.UpdateServices;

public class PublishEventUpdateService : EventPublisher, IPublishEventUpdateService
{
    public PublishEventUpdateService(RabbitMqConfig config) : base(config.QueueEventUpdate, config.HostName, config.Port)
    {
        
    }
}