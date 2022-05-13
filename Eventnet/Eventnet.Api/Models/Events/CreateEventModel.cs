using System.ComponentModel.DataAnnotations;

namespace Eventnet.Api.Models.Events;

public record CreateEventModel([Required] EventInfoModel Info, IFormFile[]? Photos);