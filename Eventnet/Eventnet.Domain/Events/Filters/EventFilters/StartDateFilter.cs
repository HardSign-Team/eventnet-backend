using Eventnet.DataAccess;
using Eventnet.Domain.Events.Filters.Data;

namespace Eventnet.Domain.Events.Filters.EventFilters;

public class StartDateFilter : BaseDateFilter
{
    public StartDateFilter(DateTime borderDate, DateEquality dateEquality) : base(borderDate, dateEquality)
    {
    }

    public override bool Filter(EventEntity entity) => Filter(entity.StartDate);
}