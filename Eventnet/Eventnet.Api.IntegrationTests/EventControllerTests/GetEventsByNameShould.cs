using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoFixture;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using Eventnet.DataAccess.Entities;
using Eventnet.TestsUtils;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.EventControllerTests;

public class GetEventsByNameShould : EventApiTestsBase
{
    [SetUp]
    public void Setup()
    {
        ApplyToDb(Utilities.ReinitializeDbForTests);
    }

    [Test]
    public async Task ResponseCode404_WhenNullName()
    {
        var request = CreateDefaultRequest(null);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ResponseCode400_WhenEmptyName()
    {
        var request = CreateDefaultRequest(" ");

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ResponseCode200_WhenNotFoundSimilarEvents()
    {
        const int maxCount = 3;
        var fixture = new Fixture();
        var events = new[]
            {
                "AAA",
                "aaa",
                "Aaa",
                "AAa",
                "AaA",
                "aAa",
                "aAA"
            }
            .Select(fixture.CreateEventWithName)
            .ToArray();
        SaveEvents(events);
        var request = CreateDefaultRequest("AAA", maxCount);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<EventNameListModel>() ?? throw new Exception();
        result.TotalCount.Should().Be(maxCount);
        result.Models.Should().HaveCount(maxCount);
    }

    private void SaveEvents(IEnumerable<EventEntity> events)
    {
        ApplyToDb(context =>
        {
            context.Events.AddRange(events);
            context.SaveChanges();
        });
    }

    private HttpRequestMessage CreateDefaultRequest(string? name, int maxCount = 10)
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsByNameUri(name, maxCount);
        request.Headers.Add("Accept", "application/json");
        return request;
    }
}