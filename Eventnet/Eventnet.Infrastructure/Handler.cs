using System.Collections.Concurrent;

namespace Eventnet.Infrastructure;

public class Handler
{
    private readonly ConcurrentDictionary<Guid, SaveEventResult> saveEventInformation = new();

    public void Update(Guid id, SaveEventResult value)
    {
        saveEventInformation[id] = value;
    }

    public bool TryGetValue(Guid id, out SaveEventResult result) => saveEventInformation.TryGetValue(id, out result);
}