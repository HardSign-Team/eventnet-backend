using Eventnet.Domain.Events.Filters.Data;

namespace Eventnet.Domain.Events.Filters.EventFilters;

public class StartDateFilter : BaseDateFilter
{
    public StartDateFilter(DateTime borderDate, DateEquality dateEquality) : base(borderDate, dateEquality)
    {
    }

    public override bool Filter(Event entity) => Filter(entity.StartDate);
}