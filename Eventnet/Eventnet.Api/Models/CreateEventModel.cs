using Eventnet.Domain.Events.Filters.Data;

namespace Eventnet.Models;

public record CreateEventModel(
    string UserId,
    string Name,
    string? Description,
    DateTime StartAt,
    DateTime? EndAt,
    Location Location);