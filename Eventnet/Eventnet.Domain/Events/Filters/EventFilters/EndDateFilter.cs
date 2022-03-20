using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events.Filters.Data;

namespace Eventnet.Domain.Events.Filters.EventFilters;

public class EndDateFilter : BaseDateFilter
{
    public EndDateFilter(DateTime borderDate, DateEquality dateEquality) : base(borderDate, dateEquality)
    {
    }

    public override bool Filter(EventEntity entity) => entity.EndDate.HasValue && Filter(entity.EndDate.Value);
}