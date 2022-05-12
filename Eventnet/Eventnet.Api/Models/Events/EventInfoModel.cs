using Eventnet.Domain.Events;

namespace Eventnet.Api.Models.Events;

public record EventInfoModel(
    Guid EventId,
    DateTime StartDate,
    DateTime? EndDate,
    string Name,
    string? Description,
    Location Location,
    string[] Tags);