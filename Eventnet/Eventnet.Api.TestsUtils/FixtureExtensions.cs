using AutoFixture;
using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.TestsUtils;

public static class FixtureExtensions
{
    public static EventEntity CreateEventWithName(this Fixture fixture, string name)
    {
        return fixture.Build<EventEntity>().With(x => x.Name, name).Create();
    }
    
    public static EventEntity CreateEventAt(this Fixture fixture, LocationEntity location)
    {
        return fixture
            .Build<EventEntity>()
            .With(x => x.Location, location)
            .Create();
    }

    public static EventEntity CreateEventStartedAt(this Fixture fixture, DateTime startDate)
    {
        return fixture.Build<EventEntity>().With(x => x.StartDate, startDate).Create();
    }

    public static EventEntity CreateEventEndedAt(this Fixture fixture, DateTime? endDate)
    {
        return fixture.Build<EventEntity>().With(x => x.EndDate, endDate).Create();
    }

    public static EventEntity CreateEventWithOwner(this Fixture fixture, string ownerId)
    {
        return new EventEntity(Guid.NewGuid(),
            ownerId,
            fixture.Create<DateTime>(),
            fixture.Create<DateTime?>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<LocationEntity>()
        );
    }
}