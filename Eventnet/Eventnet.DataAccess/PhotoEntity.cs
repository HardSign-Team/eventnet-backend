namespace Eventnet.DataAccess;

public class PhotoEntity
{
    public Guid Id { get; set; }
    
    public Guid EventId { get; set; }

    public PhotoEntity(Guid id,  Guid eventId)
    {
        Id = id;
        EventId = eventId;
    }
}