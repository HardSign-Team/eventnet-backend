using AutoFixture;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events;

namespace Eventnet.TestsUtils;

public static class FixtureExtensions
{
    public static EventEntity CreateEventWithName(this Fixture fixture, string name)
    {
        return fixture.Build<EventEntity>().With(x => x.Name, name).Create();
    }

    public static Event CreateEventAt(this Fixture fixture, Location location)
    {
        return fixture
            .Build<Event>()
            .With(x => x.Location, location)
            .Create();
    }

    public static Event CreateEventStartedAt(this Fixture fixture, DateTime startDate)
    {
        return fixture.Build<Event>().With(x => x.StartDate, startDate).Create();
    }

    public static Event CreateEventEndedAt(this Fixture fixture, DateTime? endDate)
    {
        return fixture.Build<Event>().With(x => x.EndDate, endDate).Create();
    }

    public static Event CreateEventWithOwner(this Fixture fixture, string ownerId)
    {
        return new Event(Guid.NewGuid(),
            ownerId,
            fixture.Create<DateTime>(),
            fixture.Create<DateTime?>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<Location>());
    }
    
    public static Event CreateEventWithTags(this Fixture fixture, Tag[] tags)
    {
        return fixture.Build<Event>().With(x => x.Tags, tags).Create();
    }
}