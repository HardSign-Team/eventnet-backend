namespace Eventnet.Domain.Events.Filters;

public interface IEventsFilter
{
    IEnumerable<Event> Filter(IEnumerable<Event> query);
}