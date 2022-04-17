namespace Eventnet.Domain.Events.Filters.EventFilters;

public class TagsFilter : IEventFilter
{
    private readonly HashSet<int> tagsIds;

    public TagsFilter(int[] tagsIds)
    {
        this.tagsIds = tagsIds.ToHashSet();
    }

    public bool Filter(Event entity) => tagsIds.Overlaps(entity.Tags.Select(x => x.Id));
}