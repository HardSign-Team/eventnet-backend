using Microsoft.Extensions.Caching.Memory;

namespace Eventnet.Infrastructure;

public class Handler
{
    private readonly IMemoryCache cache;

    public Handler(IMemoryCache cache)
    {
        this.cache = cache;
    }
    
    public bool ContainsGuid(Guid id) => cache.TryGetValue(id, out _);

    public void Update(Guid id, SaveEventResult value)
    {
        cache.Set(id, value, TimeSpan.FromMinutes(1));
    }
    
    public bool TryGetValue(Guid id, out SaveEventResult result) => cache.TryGetValue(id, out result);
}