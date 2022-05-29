using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using Eventnet.Api.UnitTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.EventControllerTests;

public class GetEventsByIdsShould : EventApiTestsBase
{
    [Test]
    public async Task ResponseCode200_WhenInvalidGuid()
    {
        var guids = new[] { Guid.Empty };

        var response = await PostAsync(guids);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = response.ReadContentAs<List<EventViewModel>>();
        result.Should().BeEmpty();
    }

    [Test]
    public async Task ResponseCode200_WhenMultipleIds()
    {
        AddEvents();
        var guids = ApplyToDb(context => context.Events.Select(x => x.Id).ToArray());

        var response = await PostAsync(guids);

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

        var response = await PostAsync(guids.Union(notExists).ToArray());

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

    private async Task<HttpResponseMessage> PostAsync(Guid[] guids)
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = BuildEventsByIdsUri();
        request.Headers.Add("Accept", "application/json");

        return await HttpClient.PostAsJsonAsync(request.RequestUri, new EventIdsListModel(guids));
    }
}