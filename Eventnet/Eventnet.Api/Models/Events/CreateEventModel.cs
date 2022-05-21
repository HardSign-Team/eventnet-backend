using System.ComponentModel.DataAnnotations;

namespace Eventnet.Api.Models.Events;

public record CreateEventModel(
    [Required] Guid EventId,
    [Required] DateTime StartDate,
    DateTime? EndDate,
    [Required] string Name,
    [Required] string Description,
    [Required] [Range(-90, 90)] double Longitude,
    [Required] [Range(-90, 90)] double Latitude,
    string[]? Tags,
    IFormFile[]? Photos);