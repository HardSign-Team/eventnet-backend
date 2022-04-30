namespace Eventnet.DataAccess.Entities;

public class SubscriptionEntity
{
    public Guid EventId { get; }
    public Guid UserId { get; }
    public DateTime SubscriptionDate { get; }

    public SubscriptionEntity(Guid eventId, Guid userId, DateTime subscriptionDate)
    {
        EventId = eventId;
        UserId = userId;
        SubscriptionDate = subscriptionDate;
    }
}