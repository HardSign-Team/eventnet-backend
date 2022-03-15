using Eventnet.Domain.Events.Filters.Data;

namespace Eventnet.Models;

public record Event(
    Guid Id,
    string OwnerId,
    DateTime StartDate,
    DateTime? EndDate,
    string Name,
    string Description,
    Location Location);