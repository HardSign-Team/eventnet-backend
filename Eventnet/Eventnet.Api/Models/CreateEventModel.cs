using System.ComponentModel.DataAnnotations;

namespace Eventnet.Models;

public record CreateEventModel(
    [Required] string UserId,
    [Required] string Name,
    string? Description,
    [Required] DateTime StartAt,
    DateTime EndAt,
    [Required] Location Location);