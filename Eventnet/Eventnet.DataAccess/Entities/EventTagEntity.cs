namespace Eventnet.DataAccess.Entities;

public class EventTagEntity
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local EFCore auto-set id
    public Guid EventId { get; }
    public int TagId { get; }

    public EventTagEntity(Guid eventId, int tagId)
    {
        EventId = eventId;
        TagId = tagId;
    }
}