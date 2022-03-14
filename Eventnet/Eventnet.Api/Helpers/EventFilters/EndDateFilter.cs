using Eventnet.DataAccess;

namespace Eventnet.Helpers.EventFilters;

public class EndDateFilter : BaseDateFilter
{
    public EndDateFilter(DateTime borderDate, DateEquality dateEquality) : base(borderDate, dateEquality)
    {
    }

    public override bool Filter(EventEntity entity) => entity.EndDate.HasValue && Filter(entity.EndDate.Value);
}