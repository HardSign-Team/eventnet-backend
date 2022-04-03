using AutoFixture;
using Eventnet.Domain.Selectors;

namespace Eventnet.Api.TestsUtils;

public static class EventNameFixtureExtensions
{
    public static EventName CreateEventName(this Fixture fixture, string name)
    {
        return fixture.Build<EventName>().With(x => x.Name, name).Create();
    }
}