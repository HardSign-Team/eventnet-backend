namespace Eventnet.Api.Models.Events;

public record CreateEventModel(EventInfoModel Info, IFormFile[] Photos);