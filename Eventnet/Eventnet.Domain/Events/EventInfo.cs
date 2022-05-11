namespace Eventnet.Domain.Events;

public record EventInfo(
    Guid EventId,
    string OwnerId,
    DateTime StartDate,
    DateTime? EndDate,
    string Name,
    string? Description,
    Location Location,
    string[] Tags)
{
    // ReSharper disable once UnusedMember.Local
    private EventInfo() : this(Guid.Empty,
        string.Empty,
        DateTime.Now,
        DateTime.Now,
        string.Empty,
        string.Empty,
        new Location(0, 0),
        Array.Empty<string>())
    {
    }
}