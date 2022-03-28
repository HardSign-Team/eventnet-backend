namespace Eventnet.Models;

public record CreateEventModel(
    Guid Id,
    string OwnerId,
    DateTime StartDate,
    DateTime? EndDate,
    string Name,
    string? Description,
    Location Location,
    IFormFile[] Photos);