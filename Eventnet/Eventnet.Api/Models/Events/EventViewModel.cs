using Eventnet.Api.Models.Tags;

namespace Eventnet.Api.Models.Events;

public record EventViewModel(
    Guid Id,
    string OwnerId,
    DateTime StartDate,
    DateTime? EndDate,
    TagNameModel[] Tags,
    int TotalSubscriptions);