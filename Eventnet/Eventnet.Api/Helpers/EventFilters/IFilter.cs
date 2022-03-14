namespace Eventnet.Helpers.EventFilters;

public interface IFilter<in TEntity>
{
    bool Filter(TEntity entity);
}