using Eventnet.DataAccess;

namespace Eventnet.Helpers.EventFilters;

public class OwnerFilter : IEventFilter
{
    private readonly string ownerId;

    public OwnerFilter(string ownerId)
    {
        this.ownerId = ownerId;
    }

    public bool Filter(EventEntity entity) => entity.OwnerId == ownerId;
}