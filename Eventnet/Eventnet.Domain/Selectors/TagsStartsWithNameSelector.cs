namespace Eventnet.Domain.Selectors;

public class TagsStartsWithNameSelector : ISelector<TagName>
{
    private readonly string name;

    public TagsStartsWithNameSelector(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"Expected non empty and non-nullable {nameof(name)}, but actually {name}");
        this.name = name.Trim();
    }

    public IEnumerable<TagName> Select(IEnumerable<TagName> query, int maxCount)
    {
        return query
            .Where(x => x.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
            .Take(maxCount);
    }
}