using Eventnet.Domain.Events;

namespace Eventnet.Infrastructure.UpdateServices;

public record RabbitMqUpdateMessage(Guid EventId, EventInfo? Event, List<RabbitMqPhoto> Photos, Guid[] PhotosIdToDelete);