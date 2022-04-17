using Eventnet.Api.Models.Filtering;
using Eventnet.Domain.Events.Filters.EventFilters;

namespace Eventnet.Api.Helpers.EventFilterFactories;

public class TagsFilterFactory : BaseFilterFactory<TagsFilterModel>
{
    protected override TagsFilterModel? GetProperty(EventsFilterModel model) => model.Tags;

    protected override IEventFilter Create(TagsFilterModel property) => new TagsFilter(property.TagsIds);
}