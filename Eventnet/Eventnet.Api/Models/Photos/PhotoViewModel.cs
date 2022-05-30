namespace Eventnet.Api.Models.Photos;

public record PhotoViewModel(Guid EventId, Guid PhotoId, string Url);