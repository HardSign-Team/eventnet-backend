namespace Eventnet.Api.Models.Filtering;

public class EventsFilterModel
{
    public DateFilterModel? StartDate { get; init; }
    public DateFilterModel? EndDate { get; init; }
    public OwnerFilterModel? Owner { get; init; }
    public LocationFilterModel? RadiusLocation { get; init; }
    public TagsFilterModel? Tags { get; init; }
}