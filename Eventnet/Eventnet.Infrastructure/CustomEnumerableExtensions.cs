namespace Eventnet.Infrastructure;

public static class CustomEnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        if (enumerable == null)
            throw new ArgumentNullException(nameof(enumerable));
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        foreach (var item in enumerable)
        {
            action(item);
        }
    }
}