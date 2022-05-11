using Eventnet.Api.Models.Marks;
using Eventnet.Api.Models.Tags;

namespace Eventnet.Api.Models.Events;

public record EventViewModel(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Description,
    LocationViewModel Location,
    DateTime StartDate,
    DateTime? EndDate,
    TagNameViewModel[] Tags,
    int TotalSubscriptions,
    MarksCountViewModel Marks);