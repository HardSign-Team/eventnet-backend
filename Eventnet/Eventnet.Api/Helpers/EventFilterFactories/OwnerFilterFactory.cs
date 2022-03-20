using Eventnet.Api.Models;
using Eventnet.Domain.Events.Filters.EventFilters;

namespace Eventnet.Api.Helpers.EventFilterFactories;

public class OwnerFilterFactory : BaseFilterFactory<OwnerFilterModel>
{
    protected override OwnerFilterModel? GetProperty(EventsFilterModel model) => model.Owner;

    protected override IEventFilter Create(OwnerFilterModel property) => new OwnerFilter(property.OwnerId);
}