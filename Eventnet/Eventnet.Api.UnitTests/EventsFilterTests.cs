using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Eventnet.Api.Helpers.EventFilterFactories;
using Eventnet.Api.Models.Filtering;
using Eventnet.Api.Services.Filters;
using Eventnet.Domain.Events;
using Eventnet.Domain.Events.Filters;
using Eventnet.Domain.Events.Filters.Data;
using Eventnet.TestsUtils;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.UnitTests;

public class EventsFilterTests
{
    [Test]
    public void Filter_ShouldFilterLocation()
    {
        var fixture = new Fixture();
        var events = new[]
        {
            fixture.CreateEventAt(new Location(56.840234511156446, 60.616096578611625)),
            fixture.CreateEventAt(new Location(56.84391149402939, 60.65316741081937)),
            fixture.CreateEventAt(new Location(56.81781873425029, 60.61238225939552)),
            fixture.CreateEventAt(new Location(56.81787873103509, 60.54074620394152)),
            fixture.CreateEventAt(new Location(57.15411119263984, 65.66351129051274)),
            fixture.CreateEventAt(new Location(51.15781435465198, 71.46714484897487)),
            fixture.CreateEventAt(new Location(55.75982632261345, 37.61909029735618)),
            fixture.CreateEventAt(new Location(45.432157465480515, 40.55582995809704)),
            fixture.CreateEventAt(new Location(56.114515134214784, 69.52740360680868))
        };
        var filterModel = new EventsFilterModel
        {
            RadiusLocation = new LocationFilterModel(new Location(56.83541102242807, 60.61150834516072), 25_000)
        };
        var sut = CreateDefaultService(filterModel);
        var rnd = new Random();

        var filtered = sut.Filter(events.OrderBy(_ => rnd.Next())).ToArray();

        filtered.Should().Contain(events[..4]);
        filtered.Should().HaveCount(4);
    }

    [TestCaseSource(nameof(GetFilterStartDateCases))]
    public void Filter_ShouldFilterStartDate(
        IEnumerable<Event> events,
        DateFilterModel model,
        ICollection<Event> expected)
    {
        var filterModel = new EventsFilterModel
        {
            StartDate = model
        };
        var sut = CreateDefaultService(filterModel);
        var rnd = new Random();

        var filtered = sut.Filter(events.OrderBy(_ => rnd.Next())).ToArray();

        filtered.Should().Contain(expected);
        filtered.Should().HaveCount(expected.Count);
    }

    [TestCaseSource(nameof(GetFilterEndDateCases))]
    public void Filter_ShouldFilterEndDate(
        IEnumerable<Event> events,
        DateFilterModel model,
        ICollection<Event> expected)
    {
        var filterModel = new EventsFilterModel
        {
            EndDate = model
        };
        var sut = CreateDefaultService(filterModel);
        var rnd = new Random();

        var filtered = sut.Filter(events.OrderBy(_ => rnd.Next())).ToArray();

        filtered.Should().Contain(expected);
        filtered.Should().HaveCount(expected.Count);
    }

    [Test]
    public void Filter_ShouldFilterByOwner()
    {
        var fixture = new Fixture();
        var ownerId = Guid.NewGuid();
        var events = new[]
        {
            fixture.CreateEventWithOwner(ownerId),
            fixture.CreateEventWithOwner(ownerId),
            fixture.CreateEventWithOwner(ownerId),
            fixture.CreateEventWithOwner(Guid.NewGuid()),
            fixture.CreateEventWithOwner(Guid.NewGuid())
        };
        var filterModel = new EventsFilterModel
        {
            Owner = new OwnerFilterModel(ownerId)
        };
        var sut = CreateDefaultService(filterModel);

        var filteredEvents = sut.Filter(events).ToArray();

        filteredEvents.Should().Contain(events[..3]);
        filteredEvents.Should().HaveCount(3);
    }

    [Test]
    public void Filter_ShouldFilterByTags()
    {
        var fixture = new Fixture();
        var tags = new[]
        {
            new Tag(1, "gay"),
            new Tag(2, "boynextdoor"),
            new Tag(3, "dohomeworkpls"),
            new Tag(4, "bonjourнаблевал"),
            new Tag(5, "bonanнаблевал"),
            new Tag(6, "иваннаблевал"),
            new Tag(7, "максимнаблевал"),
            new Tag(8, "артемнаблевал"),
            new Tag(9, "мишанаблевал")
        };
        var events = new[]
        {
            fixture.CreateEventWithTags(new[] { tags[0], tags[1], tags[2] }),
            fixture.CreateEventWithTags(new[] { tags[2], tags[1], tags[6] }),
            fixture.CreateEventWithTags(new[] { tags[1], tags[2], tags[7] }),
            fixture.CreateEventWithTags(new[] { tags[3], tags[5], tags[4] }),
            fixture.CreateEventWithTags(new[] { tags[7], tags[6], tags[5] })
        };
        var filterModel = new EventsFilterModel
        {
            Tags = new TagsFilterModel(new[] { tags[1].Id, tags[2].Id })
        };
        var sut = CreateDefaultService(filterModel);

        var filteredEvents = sut.Filter(events).ToArray();

        filteredEvents.Should().Equal(events[..3]);
    }

    public static IEnumerable<TestCaseData> GetFilterStartDateCases()
    {
        var fixture = new Fixture();
        var store = new[]
        {
            fixture.CreateEventStartedAt(new DateTime(2002, 1, 31, 12, 00, 00)),
            fixture.CreateEventStartedAt(new DateTime(2002, 1, 31, 14, 18, 0)),
            fixture.CreateEventStartedAt(new DateTime(2015, 2, 1)),
            fixture.CreateEventStartedAt(new DateTime(2015, 12, 31)),
            fixture.CreateEventStartedAt(new DateTime(2022, 2, 2)),
            fixture.CreateEventStartedAt(new DateTime(2022, 2, 24))
        };

        yield return new TestCaseData(store,
                new DateFilterModel(new DateTime(2016, 1, 1), DateEquality.Before),
                store[..4])
            .SetName("Filter before start date");

        yield return new TestCaseData(store,
                new DateFilterModel(new DateTime(2016, 1, 1), DateEquality.After),
                store[4..])
            .SetName("Filter after start date");

        yield return new TestCaseData(store,
                new DateFilterModel(new DateTime(2002, 1, 31), DateEquality.SameDay),
                store[..2])
            .SetName("Filter same day start date");
    }

    private static IEventsFilter CreateDefaultService(EventsFilterModel filterModel)
    {
        var mapper = new EventFilterMapper(new IEventFilterFactory[]
        {
            new LocationFilterFactory(),
            new StartDateFilterFactory(),
            new EndDateFilterFactory(),
            new OwnerFilterFactory(),
            new TagsFilterFactory()
        });
        return mapper.Map(filterModel);
    }

    private static IEnumerable<TestCaseData> GetFilterEndDateCases()
    {
        var fixture = new Fixture();
        var store = new[]
        {
            fixture.CreateEventEndedAt(new DateTime(2002, 1, 31, 12, 00, 00)),
            fixture.CreateEventEndedAt(new DateTime(2002, 1, 31, 14, 18, 0)),
            fixture.CreateEventEndedAt(new DateTime(2015, 2, 1)),
            fixture.CreateEventEndedAt(new DateTime(2015, 12, 31)),
            fixture.CreateEventEndedAt(new DateTime(2022, 2, 2)),
            fixture.CreateEventEndedAt(new DateTime(2022, 2, 24)),
            fixture.CreateEventEndedAt(null)
        };

        yield return new TestCaseData(store,
                new DateFilterModel(new DateTime(2016, 1, 1), DateEquality.Before),
                store[..4])
            .SetName("Filter before end date");

        yield return new TestCaseData(store,
                new DateFilterModel(new DateTime(2016, 1, 1), DateEquality.After),
                store[4..6])
            .SetName("Filter after end date");

        yield return new TestCaseData(store,
                new DateFilterModel(new DateTime(2002, 1, 31), DateEquality.SameDay),
                store[..2])
            .SetName("Filter same day end date");
    }
}