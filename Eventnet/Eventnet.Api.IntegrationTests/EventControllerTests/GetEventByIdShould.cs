using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.DataAccess.Entities;
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
        var eventEntity = GetTestEvent();
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
            location = eventEntity.Location,
            tags = eventEntity.Tags,
            totalSubscriptions = eventEntity.Subscriptions.Count
        });
    }

    private EventEntity GetTestEvent()
    {
        return ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents();
            context.AddTags();

            var entity = context.Events.First();
            var tagEntities = context.Tags.Take(6).ToArray();
            var subscribers = context.Users.Take(3).ToArray();

            foreach (var subscriber in subscribers)
            {
                entity.Subscribe(subscriber);
            }
            foreach (var tag in tagEntities)
            {
                entity.AddTag(tag);
            }
            context.SaveChanges();
            return entity;
        });
    }
}