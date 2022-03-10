using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.Tests.Helpers;
using Eventnet.DataAccess;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.Tests.EventControllerTests;

public class GetEventByIdShould : EventApiTestsBase
{
    [Test]
    public async Task ResponseCode404_WhenInvalidGuid()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsByIdUri("trash");
        request.Headers.Add("Accept", "application/json");

        var response = await HttpClient.SendAsync(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.ShouldNotHaveHeader("Content-Type");
    }

    [Test]
    public async Task ResponseCode404_WhenEventNotFound()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsByIdUri(Guid.NewGuid());
        request.Headers.Add("Accept", "application/json");

        var response = await HttpClient.SendAsync(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.ShouldNotHaveHeader("Content-Type");
    }
    
    [Test]
    public async Task ResponseCode422_WhenEmptyGuid()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsByIdUri(Guid.Empty);
        request.Headers.Add("Accept", "application/json");

        var response = await HttpClient.SendAsync(request);
        
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
    
    [Test]
    public async Task ResponseCode200_WhenEventExists()
    {
        var eventEntity = ApplyToDb(context => context.Events.First());
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsByIdUri(eventEntity.Id);
        request.Headers.Add("Accept", "application/json");
        
        var response = await HttpClient.SendAsync(request);

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
}