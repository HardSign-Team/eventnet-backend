using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.EventControllerTests;

public class GetEventsByIdsShould : EventApiTestsBase
{
    [Test]
    public async Task ResponseCode404_WhenInvalidGuid()
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsByIdsUri(new [] {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });
        request.Headers.Add("Accept", "application/json");

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = response.ReadContentAs < List<EventViewModel>>();
        result.Should().BeEmpty();
    }
    
    [Test]
    public async Task ResponseCode200_WhenMultipleIds()
    {
        AddEvents();
        var guids = ApplyToDb(context => context.Events.Select(x => x.Id).ToArray());
        var request = new HttpRequestMessage(HttpMethod.Get, BuildEventsByIdsUri(guids));
        request.Headers.Add("Accept", "application/json");

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = response.ReadContentAs<List<EventViewModel>>();
        result.Should().HaveCount(guids.Length);
    }

    [Test]
    public async Task ResponseCode200_WhenSomeEventsNotExists()
    {
        AddEvents();
        var guids = ApplyToDb(context => context.Events.Select(x => x.Id).ToArray());
        var notExists = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var request = new HttpRequestMessage(HttpMethod.Get, BuildEventsByIdsUri(guids.Concat(notExists).ToArray()));
        request.Headers.Add("Accept", "application/json");

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = response.ReadContentAs<List<EventViewModel>>();
        result.Should().HaveCount(guids.Length);
    }
    
    private void AddEvents()
    {
        ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
        });
    }
}