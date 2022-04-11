using Eventnet.Api.Models.Filtering;
using Eventnet.Domain.Events.Filters.EventFilters;

namespace Eventnet.Api.Helpers.EventFilterFactories;

public class EndDateFilterFactory : BaseFilterFactory<DateFilterModel>
{
    protected override DateFilterModel? GetProperty(EventsFilterModel model) => model.EndDate;

    protected override IEventFilter Create(DateFilterModel property) =>
        new EndDateFilter(property.Border, property.DateEquality);
}