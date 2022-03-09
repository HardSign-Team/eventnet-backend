using System;
using System.Net;
using System.Net.Http;
using Eventnet.DataAccess;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.Tests.EventControllerTests;

public class GetEventByIdShould : EventApiTestsBase
{
    [Test]
    public void ResponseCode404_WhenInvalidGuid()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsByIdUri("trash");
        request.Headers.Add("Accept", "application/json");

        var response = HttpClient.Send(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.ShouldNotHaveHeader("Content-Type");
    }

    [Test]
    public void ResponseCode404_WhenEventNotFound()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsByIdUri(Guid.NewGuid());
        request.Headers.Add("Accept", "application/json");

        var response = HttpClient.Send(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.ShouldNotHaveHeader("Content-Type");
    }
    
    [Test]
    public void ResponseCode422_WhenEmptyGuid()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsByIdUri(Guid.Empty);
        request.Headers.Add("Accept", "application/json");

        var response = HttpClient.Send(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
    
    [Test]
    public void ResponseCode200_WhenEventExists()
    {
        var eventEntity = CreateEvent();
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsByIdUri(eventEntity.Id);
        request.Headers.Add("Accept", "application/json");
        
        var response = HttpClient.Send(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
        response.ShouldHaveJsonContentEquivalentTo(new
        {
            id = eventEntity.Id,
            ownerId = eventEntity.OwnerId,
            name = eventEntity.Name,
            description = eventEntity.Description,
            startDate = eventEntity.StartDate,
            endDate = eventEntity.EndDate,
            locationEntity = eventEntity.Location
        });
    }

    private EventEntity CreateEvent()
    {
        return new EventEntity(Guid.NewGuid(),
            "",
            new DateTime(2022, 02, 24),
            null,
            "Event",
            "No description",
            new LocationEntity(49d, 32d));
    }
}