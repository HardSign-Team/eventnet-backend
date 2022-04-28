using System;
using AutoFixture;
using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.IntegrationTests.Helpers;

public class EventEntityMother
{
    public static EventEntity CreateEventEntity()
    {
        var fixture = new Fixture();
        return fixture.Create<EventEntity>();
    }

    public static EventEntity CreateEventEntity(string userId, DateTime startDate, DateTime? endDate = null)
    {
        var fixture = new Fixture();
        return new EventEntity(Guid.NewGuid(),
            userId,
            startDate,
            endDate,
            fixture.Create<string>(),
            fixture.Create<string>(),
            fixture.Create<LocationEntity>());
    }
}