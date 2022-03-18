using Eventnet.DataAccess;

namespace Eventnet.Domain.Events.Filters.EventFilters;

public class OwnerFilter : IEventFilter
{
    private readonly string ownerId;

    public OwnerFilter(string ownerId)
    {
        this.ownerId = ownerId;
    }

    public bool Filter(EventEntity entity) => entity.OwnerId == ownerId;
}