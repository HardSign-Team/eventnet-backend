using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using Eventnet.Api.Models.Marks;
using Eventnet.Api.Models.Tags;
using Eventnet.Api.UnitTests.Helpers;
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
        request.RequestUri = BuildEventByIdUri("trash");
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
        request.RequestUri = BuildEventByIdUri(Guid.NewGuid());
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
        request.RequestUri = BuildEventByIdUri(Guid.Empty);
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
        request.RequestUri = BuildEventByIdUri(eventEntity.Id);
        request.Headers.Add("Accept", "application/json");

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
        var model = response.ReadContentAs<EventViewModel>();
        model.Should().BeEquivalentTo(new EventViewModel(eventEntity.Id,
            eventEntity.OwnerId,
            eventEntity.Name,
            eventEntity.Description,
            new LocationViewModel(eventEntity.Location.Latitude, eventEntity.Location.Longitude),
            eventEntity.StartDate,
            eventEntity.EndDate,
            eventEntity.Tags.Select(x => new TagNameViewModel(x.Id, x.Name)).ToList(),
            eventEntity.Subscriptions.Count,
            new MarksCountViewModel(2, 1)));
    }

    private EventEntity GetTestEvent()
    {
        return ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            context.AddTags();

            var entity = context.Events.First();

            AddSubscribers(entity, context.Users.Take(3).ToArray());
            AddTags(entity, context.Tags.Take(6).ToArray());

            var marks = AddMarks(entity, context.Users.Take(3).ToArray());
            context.Marks.AddRange(marks);
            context.SaveChanges();
            return entity;
        });
    }

    private static IEnumerable<MarkEntity> AddMarks(EventEntity eventEntity, UserEntity[] users)
    {
        yield return eventEntity.Like(users[0]);
        yield return eventEntity.Like(users[1]);
        yield return eventEntity.Dislike(users[2]);
    }

    private static void AddSubscribers(EventEntity eventEntity, IEnumerable<UserEntity> users)
    {
        foreach (var user in users)
            eventEntity.Subscribe(user);
    }

    private static void AddTags(EventEntity eventEntity, IEnumerable<TagEntity> tags)
    {
        foreach (var tag in tags)
            eventEntity.AddTag(tag);
    }
}