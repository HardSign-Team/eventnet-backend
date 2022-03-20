namespace Eventnet.Domain.Selectors;

public interface ISelector<T>
{
    IEnumerable<T> Select(IEnumerable<T> query, int maxCount);
}