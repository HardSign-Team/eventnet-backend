using Microsoft.Extensions.Caching.Memory;

namespace Eventnet.Infrastructure;

public class EventSaveHandler
{
    private readonly IMemoryCache cache;

    public EventSaveHandler(IMemoryCache cache)
    {
        this.cache = cache;
    }
    
    public bool IsHandling(Guid id) => cache.TryGetValue(id, out _);

    public void Update(Guid id, SaveEventResult value)
    {
        cache.Set(id, value, TimeSpan.FromMinutes(1));
    }
    
    public bool TryGetValue(Guid id, out SaveEventResult result) => cache.TryGetValue(id, out result);
}