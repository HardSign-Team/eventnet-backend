namespace Eventnet.Domain.Selectors;

public class TagsStartsWithNameSelector : ISelector<TagName>
{
    private readonly string name;

    public TagsStartsWithNameSelector(string? name)
    {
        name = name?.Trim();
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException($"Expected non empty and non-nullable {nameof(name)}, but actually {name}");
        this.name = name.ToLower();
    }

    public IEnumerable<TagName> Select(IEnumerable<TagName> query, int maxCount)
    {
        return query
            .Where(x => x.Name.ToLower().StartsWith(name))
            .Take(maxCount);
    }
}