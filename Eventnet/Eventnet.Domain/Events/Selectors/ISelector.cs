namespace Eventnet.Domain.Events.Selectors;

public interface ISelector<T>
{
    IEnumerable<T> Select(IEnumerable<T> query, int maxCount);
}