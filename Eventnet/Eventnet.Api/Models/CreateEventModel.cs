using Eventnet.Domain.Events;

namespace Eventnet.Api.Models;

public record CreateEventModel(
    string UserId,
    string Name,
    string? Description,
    DateTime StartAt,
    DateTime? EndAt,
    Location Location);