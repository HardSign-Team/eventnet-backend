namespace Eventnet.DataAccess.Entities;

public class SubscriptionEntity
{
    public Guid EventId { get; }
    public string UserId { get; }
    public DateTime SubscriptionDate { get; }

    public SubscriptionEntity(Guid eventId, string userId, DateTime subscriptionDate)
    {
        EventId = eventId;
        UserId = userId;
        SubscriptionDate = subscriptionDate;
    }
}