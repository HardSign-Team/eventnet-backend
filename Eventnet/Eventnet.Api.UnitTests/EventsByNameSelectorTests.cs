using System;
using System.Linq;
using AutoFixture;
using Eventnet.Api.TestsUtils;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events.Selectors;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.UnitTests;

public class EventsByNameSelectorTests
{
    [Test]
    public void Select_ShouldReturnEmptyCollectionWhenEmptyInput()
    {
        var sut = CreateSut("Fat cock");
        var events = Array.Empty<EventEntity>();
        
        var result = sut.Select(events, 100);

        result.Should().HaveCount(0);
    }

    [Test]
    public void Select_ShouldReturnSimilarEvents()
    {
        var sut = CreateSut("Fat cock sucking");
        var fixture = new Fixture();
        var events = new[]
        {
            fixture.CreateEventWithName("Fat dick sucking"),
            fixture.CreateEventWithName("Gay party"),
            fixture.CreateEventWithName("Mad cock suckicng"),
            fixture.CreateEventWithName("Master cock sucking")
        };
        
        var result = sut.Select(events, 100).ToArray();

        result.Should().Contain(new[]
        {
            events[0],
            events[2],
            events[3],
        });
        result.Should().HaveCount(3);
    }

    [Test]
    public void Select_ShouldReturnEmptyCollection_WhenSimilarEventsNotFound()
    {
        var sut = CreateSut("Maksim-Mathter");
        var fixture = new Fixture();
        var events = new[]
        {
            fixture.CreateEventWithName("Ivan gay"),
            fixture.CreateEventWithName("Bonjour gay"),
            fixture.CreateEventWithName("Bonan gay"),
            fixture.CreateEventWithName("Vasya gay")
        };
        
        var result = sut.Select(events, 100).ToArray();

        result.Should().HaveCount(0);
    }

    [Test]
    public void Select_ShouldReturnTrimmedCollection_WhenFoundMoreThanMaxCount()
    {
        var sut = CreateSut("AAAB");
        var fixture = new Fixture();
        var events = new[]
        {
            fixture.CreateEventWithName("AAAB"),
            fixture.CreateEventWithName("AAAB"),
            fixture.CreateEventWithName("AAAB"),
            fixture.CreateEventWithName("AAAB"),
            fixture.CreateEventWithName("AAAB"),
            fixture.CreateEventWithName("AAAB"),
        };
        
        var result = sut.Select(events, 3).ToArray();

        result.Should().Contain(events[..3]);
        result.Should().HaveCount(3);
    }

    
    private EventsByNameSelector CreateSut(string name)
    {
        return new EventsByNameSelector(name);
    }
}