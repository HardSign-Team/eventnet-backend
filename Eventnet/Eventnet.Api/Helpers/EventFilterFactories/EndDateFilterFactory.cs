using Eventnet.Helpers.EventFilters;
using Eventnet.Models;

namespace Eventnet.Helpers.EventFilterFactories;

public class EndDateFilterFactory : BaseFilterFactory<DateFilterModel>
{
    protected override DateFilterModel? GetProperty(FilterEventsModel model) => model.EndDate;

    protected override IEventFilter Create(DateFilterModel property) =>
        new EndDateFilter(property.Border, property.DateEquality);
}