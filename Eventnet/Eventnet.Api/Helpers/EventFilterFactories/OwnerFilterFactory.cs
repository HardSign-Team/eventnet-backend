using Eventnet.Domain.Events.Filters.EventFilters;
using Eventnet.Models;

namespace Eventnet.Helpers.EventFilterFactories;

public class OwnerFilterFactory : BaseFilterFactory<OwnerFilterModel>
{
    protected override OwnerFilterModel? GetProperty(EventsFilterModel model) => model.Owner;

    protected override IEventFilter Create(OwnerFilterModel property) => new OwnerFilter(property.OwnerId);
}