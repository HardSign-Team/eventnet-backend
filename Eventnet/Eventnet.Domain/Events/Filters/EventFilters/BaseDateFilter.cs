using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events.Filters.Data;

namespace Eventnet.Domain.Events.Filters.EventFilters;

public abstract class BaseDateFilter : IEventFilter
{
    private readonly DateTime borderDate;
    private readonly DateEquality dateEquality;

    public BaseDateFilter(DateTime borderDate, DateEquality dateEquality)
    {
        this.borderDate = borderDate;
        this.dateEquality = dateEquality;
    }

    public abstract bool Filter(EventEntity entity);

    protected bool Filter(DateTime date)
    {
        return dateEquality switch
        {
            DateEquality.Before  => date < borderDate,
            DateEquality.SameDay => date.Date == borderDate.Date,
            DateEquality.After   => date > borderDate,
            _                    => throw new ArgumentOutOfRangeException(nameof(date), $"Unexpected {date}")
        };
    }
}