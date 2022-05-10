using Eventnet.Domain.Events;

namespace Eventnet.Api.Models.Events;

public record CreateEventModel(
    Guid Id,
    string OwnerId,
    DateTime StartDate,
    DateTime? EndDate,
    string Name,
    string? Description,
    Location Location,
    string[] Tags,
    IFormFile[] Photos);