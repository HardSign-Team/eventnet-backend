// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Eventnet.DataAccess.Entities;

public class MarkEntity
{
    public Guid UserId { get; }
    public Guid EventId { get; }
    public bool IsLike { get; private set; }
    public DateTime Date { get; private set; }

    public MarkEntity(Guid userId, Guid eventId, bool isLike, DateTime date)
    {
        IsLike = isLike;
        UserId = userId;
        EventId = eventId;
        Date = date;
    }

    public void Like()
    {
        if (!IsLike)
        {
            IsLike = true;
            Date = DateTime.Now;
        }
    }

    public void Dislike()
    {
        if (IsLike)
        {
            IsLike = false;
            Date = DateTime.Now;
        }
    }
}