namespace Eventnet.Domain.Events;

public record Event(
    Guid Id,
    Guid OwnerId,
    DateTime StartDate,
    DateTime? EndDate,
    string Name,
    string Description,
    Location Location)
{
    public Tag[] Tags { get; set; } = Array.Empty<Tag>();
}