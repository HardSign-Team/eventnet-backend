using Eventnet.Domain.Events;

namespace Eventnet.Api.Models.Events;

public record CreateEventModel(
    string UserId,
    string Name,
    string? Description,
    DateTime StartAt,
    DateTime? EndAt,
    Location Location);