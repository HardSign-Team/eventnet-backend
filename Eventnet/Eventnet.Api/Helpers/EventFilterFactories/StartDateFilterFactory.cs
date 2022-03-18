using Eventnet.Domain.Events.Filters.EventFilters;
using Eventnet.Models;

namespace Eventnet.Helpers.EventFilterFactories;

public class StartDateFilterFactory : BaseFilterFactory<DateFilterModel>
{
    protected override DateFilterModel? GetProperty(EventsFilterModel model) => model.StartDate;

    protected override IEventFilter Create(DateFilterModel property) =>
        new StartDateFilter(property.Border, property.DateEquality);
}