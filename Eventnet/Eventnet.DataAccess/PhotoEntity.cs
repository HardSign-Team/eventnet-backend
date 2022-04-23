// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local EFCORE
namespace Eventnet.DataAccess;

public class PhotoEntity
{
    public Guid Id { get; private set; }
    
    public Guid EventId { get; }

    public PhotoEntity(Guid id,  Guid eventId)
    {
        Id = id;
        EventId = eventId;
    }
}