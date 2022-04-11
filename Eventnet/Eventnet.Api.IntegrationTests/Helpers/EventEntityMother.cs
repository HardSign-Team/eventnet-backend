using System;
using AutoFixture;
using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.IntegrationTests.Helpers;

public class EventEntityMother
{
    public static EventEntity CreateEventEntity()
    {
        var fixture = new Fixture();
        return new EventEntity(Guid.NewGuid(),
            fixture.Create<string>(),
            fixture.Create<DateTime>(),
            fixture.Create<DateTime>(),
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<LocationEntity>());
    }
}