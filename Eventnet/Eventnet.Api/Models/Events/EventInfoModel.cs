using System.ComponentModel.DataAnnotations;
using Eventnet.Domain.Events;

namespace Eventnet.Api.Models.Events;

public record EventInfoModel(
    [Required] Guid EventId,
    [Required] DateTime StartDate,
    DateTime? EndDate,
    [Required] string Name,
    string? Description,
    [Required] Location Location,
    [Required] string[] Tags);