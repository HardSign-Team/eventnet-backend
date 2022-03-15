namespace Eventnet.Models;

public class EventsFilterModel
{
    public LocationFilterModel? RadiusLocation { get; init; }
    public DateFilterModel? StartDate { get; init; }
    public DateFilterModel? EndDate { get; init; }
    public OwnerFilterModel? Owner { get; init; }
}