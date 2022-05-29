using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure;

public record RabbitMqSaveMessage(EventInfo EventInfo, List<RabbitMqPhoto> Photos);