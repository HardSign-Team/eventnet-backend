// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local EFCORE

namespace Eventnet.DataAccess.Entities;

public class PhotoEntity
{
    public Guid EventId { get; }
    public Guid Id { get; private set; }

    public PhotoEntity(Guid id, Guid eventId)
    {
        Id = id;
        EventId = eventId;
    }
}