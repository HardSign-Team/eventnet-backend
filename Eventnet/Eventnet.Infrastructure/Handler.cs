using System.Collections.Concurrent;

namespace Eventnet.Infrastructure;

public class Handler
{
    private readonly ConcurrentDictionary<Guid, SaveEventResult> saveEventInformation = new();

    public void Update(Guid id, SaveEventResult value)
    {
        // может ли возникнуть ситуация когда нужно будет использовать TryUpdate ?
        saveEventInformation[id] = value;
    }

    public SaveEventResult GetValue(Guid id)
    {
        saveEventInformation.Remove(id, out var value);
        return value;
    }

    public bool ContainsInformationAbout(Guid id) => saveEventInformation.ContainsKey(id);
}