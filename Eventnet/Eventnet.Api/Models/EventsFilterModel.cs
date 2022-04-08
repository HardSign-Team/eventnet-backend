namespace Eventnet.Api.Models;

public class EventsFilterModel
{
    public DateFilterModel? EndDate { get; init; }
    public OwnerFilterModel? Owner { get; init; }
    public LocationFilterModel? RadiusLocation { get; init; }
    public DateFilterModel? StartDate { get; init; }
}