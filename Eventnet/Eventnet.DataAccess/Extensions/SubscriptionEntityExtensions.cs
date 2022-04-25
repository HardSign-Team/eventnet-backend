using Eventnet.DataAccess.Entities;

namespace Eventnet.DataAccess.Extensions;

public static class SubscriptionEntityExtensions
{
    public static IQueryable<SubscriptionEntity> Of(this IQueryable<SubscriptionEntity> query, UserEntity user)
    {
        return query.Where(x => x.UserId == user.Id);
    }
    
    public static IQueryable<SubscriptionEntity> For(this IQueryable<SubscriptionEntity> query, EventEntity eventEntity)
    {
        return query.Where(x => x.EventId == eventEntity.Id);
    }
}