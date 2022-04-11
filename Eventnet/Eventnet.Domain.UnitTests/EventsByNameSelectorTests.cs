using System;
using System.Linq;
using AutoFixture;
using Eventnet.Domain.Selectors;
using Eventnet.TestsUtils;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Domain.UnitTests;

public class EventsByNameSelectorTests
{
    [Test]
    public void Select_ShouldReturnEmptyCollectionWhenEmptyInput()
    {
        var sut = CreateSut("Fat cock");
        var events = Array.Empty<EventName>();
        
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
            fixture.CreateEventName("Fat dick sucking"),
            fixture.CreateEventName("Gay party"),
            fixture.CreateEventName("Mad cock suckicng"),
            fixture.CreateEventName("Master cock sucking")
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
            fixture.CreateEventName("Ivan gay"),
            fixture.CreateEventName("Bonjour gay"),
            fixture.CreateEventName("Bonan gay"),
            fixture.CreateEventName("Vasya gay")
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
            fixture.CreateEventName("AAAB"),
            fixture.CreateEventName("AAAB"),
            fixture.CreateEventName("AAAB"),
            fixture.CreateEventName("AAAB"),
            fixture.CreateEventName("AAAB"),
            fixture.CreateEventName("AAAB"),
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