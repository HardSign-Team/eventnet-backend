namespace Eventnet.Domain.Events.Filters.EventFilters;

public class OwnerFilter : IEventFilter
{
    private readonly string ownerId;

    public OwnerFilter(string ownerId)
    {
        this.ownerId = ownerId;
    }

    public bool Filter(Event entity) => entity.OwnerId == ownerId;
}