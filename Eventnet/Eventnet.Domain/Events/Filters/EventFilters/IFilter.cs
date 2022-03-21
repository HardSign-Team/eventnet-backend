namespace Eventnet.Domain.Events.Filters.EventFilters;

public interface IFilter<in TEntity>
{
    bool Filter(TEntity entity);
}