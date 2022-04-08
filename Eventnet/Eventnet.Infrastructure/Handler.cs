using Microsoft.Extensions.Caching.Memory;

namespace Eventnet.Infrastructure;

public class Handler
{
    private readonly IMemoryCache cache;

    public Handler(IMemoryCache cache)
    {
        this.cache = cache;
    }

    public void Update(Guid id, SaveEventResult value)
    {
        cache.Set(id, value, TimeSpan.FromMinutes(2));
    }
    
    public bool TryGetValue(Guid id, out SaveEventResult result)
    {
        if (!cache.TryGetValue(id, out result))
            return false;
        cache.Remove(id);
        return true;
    }
}