using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure;

public record RabbitMqMessage(EventInfo EventInfo, List<RabbitMqPhoto> Photos);