using F23.StringSimilarity;

namespace Eventnet.Domain.Selectors;

public class EventsByNameSelector : ISelector<EventName>
{
    private readonly string name;
    private readonly NormalizedLevenshtein algorithm;

    public EventsByNameSelector(string name)
    {
        this.name = name.ToLowerInvariant();
        algorithm = new NormalizedLevenshtein();
    }
    
    public IEnumerable<EventName> Select(IEnumerable<EventName> events, int maxCount)
    {
        const double acceptableSimilarity = 0.7;
        var valueTuples = events
            .Select(x => new { Event=  x, Similarity = algorithm.Similarity(x.Name.ToLowerInvariant(), name)})
            .ToArray();
        return valueTuples
            .Where(x => x.Similarity >= acceptableSimilarity)
            .OrderByDescending(x => x.Similarity)
            .Take(maxCount)
            .Select(x => x.Event);
    }
}