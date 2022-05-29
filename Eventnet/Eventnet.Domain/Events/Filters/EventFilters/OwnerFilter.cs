namespace Eventnet.Domain.Events.Filters.EventFilters;

public class OwnerFilter : IEventFilter
{
    private readonly Guid ownerId;

    public OwnerFilter(Guid ownerId)
    {
        this.ownerId = ownerId;
    }

    public bool Filter(Event entity) => entity.OwnerId == ownerId;
}