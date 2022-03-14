using Eventnet.Helpers.EventFilters;
using Eventnet.Models;

namespace Eventnet.Helpers.EventFilterFactories;

public class StartDateFilterFactory : BaseFilterFactory<DateFilterModel>
{
    protected override DateFilterModel? GetProperty(FilterEventsModel model) => model.StartDate;

    protected override IEventFilter Create(DateFilterModel property) =>
        new StartDateFilter(property.Border, property.DateEquality);
}