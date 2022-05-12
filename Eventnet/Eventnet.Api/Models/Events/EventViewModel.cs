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
    List<TagNameViewModel> Tags,
    int TotalSubscriptions,
    MarksCountViewModel Marks)
{
    private static readonly MarksCountViewModel MarksCountViewModel = new(0, 0);

    // ReSharper disable once UnusedMember.Local Used for AutoMapper
    private EventViewModel(
        Guid id,
        string ownerId,
        string name,
        string description,
        LocationViewModel location,
        DateTime startDate,
        DateTime? endDate,
        List<TagNameViewModel> tags) : this(id,
        ownerId,
        name,
        description,
        location,
        startDate,
        endDate,
        tags,
        0,
        MarksCountViewModel)
    {
    }
}