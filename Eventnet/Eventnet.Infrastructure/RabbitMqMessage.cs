using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure;

public record RabbitMqMessage(Event Event, List<RabbitMqPhoto> Photos);