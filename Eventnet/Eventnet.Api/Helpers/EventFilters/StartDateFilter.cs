using Eventnet.DataAccess;

namespace Eventnet.Helpers.EventFilters;

public class StartDateFilter : BaseDateFilter
{
    public StartDateFilter(DateTime borderDate, DateEquality dateEquality) : base(borderDate, dateEquality)
    {
    }

    public override bool Filter(EventEntity entity) => Filter(entity.StartDate);
}