using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.TestsUtils;
using Eventnet.DataAccess.Entities;
using Eventnet.DataAccess.Migrations;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.EventControllerTests;

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
        var eventEntity = EventEntityMother.CreateEventEntity();
        var tags = new[] { "A", "B", "ccc" };
        AddTags(eventEntity, tags);
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
            locationEntity = eventEntity.Location,
            tags = eventEntity.Tags,
        });
    }

    private void AddTags(EventEntity eventEntity, string[] tags)
    {
        ApplyToDb(context =>
        {
            var tagEntities = tags.Select(x => new TagEntity(x)).ToArray();
            context.Tags.AddRange(tagEntities);
            context.SaveChanges();

            eventEntity.Tags.AddRange(tagEntities);
            context.Events.Add(eventEntity);
            context.SaveChanges();
        });
    }

   
}