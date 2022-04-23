using Eventnet.Api.Models.Marks;
using Eventnet.Api.Models.Tags;

namespace Eventnet.Api.Models.Events;

public record EventViewModel(
    Guid Id,
    string OwnerId,
    string Name,
    string Description,
    LocationViewModel Location,
    DateTime StartDate,
    DateTime? EndDate,
    TagNameModel[] Tags,
    int TotalSubscriptions,
    MarksCountViewModel Marks);