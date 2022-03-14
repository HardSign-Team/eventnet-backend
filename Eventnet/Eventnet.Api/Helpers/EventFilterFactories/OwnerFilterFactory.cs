using Eventnet.Helpers.EventFilters;
using Eventnet.Models;

namespace Eventnet.Helpers.EventFilterFactories;

public class OwnerFilterFactory : BaseFilterFactory<OwnerFilterModel>
{
    protected override OwnerFilterModel? GetProperty(FilterEventsModel model) => model.Owner;

    protected override IEventFilter Create(OwnerFilterModel property) => new OwnerFilter(property.OwnerId);
}