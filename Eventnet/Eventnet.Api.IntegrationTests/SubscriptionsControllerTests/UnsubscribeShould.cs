using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Subscriptions;
using Eventnet.Api.UnitTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.SubscriptionsControllerTests;

public class UnsubscribeShould : SubscriptionsApiTestsBase
{
    [Test]
    public async Task Response401_WhenUserNotAuthorized()
    {
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });
        var request = BuildUnsubscribeRequest(entity.Id);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Response404_WhenEventNotFound()
    {
        var (_, client) = await CreateAuthorizedClient();
        var request = BuildUnsubscribeRequest(Guid.Empty);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Response409_WhenEventHasNotEndDate()
    {
        var (_, client) = await CreateAuthorizedClient();
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            var user = context.Users.First();
            var entity = EventEntityMother.CreateEventEntity(user.Id, DateTime.Now.AddDays(-10));
            context.Events.Add(entity);
            context.SaveChanges();
            return entity;
        });
        var request = BuildUnsubscribeRequest(entity.Id);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task Response409_WhenEventEnded()
    {
        var (_, client) = await CreateAuthorizedClient();
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            var user = context.Users.First();
            var now = DateTime.Now;
            var entity = EventEntityMother.CreateEventEntity(user.Id, now.AddDays(-10), now.AddDays(-5));
            context.Events.Add(entity);
            context.SaveChanges();
            return entity;
        });
        var request = BuildUnsubscribeRequest(entity.Id);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(10)]
    public async Task Response200_WhenUnsubscribeNTimes(int n)
    {
        var (user, client) = await CreateAuthorizedClient();
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            var entity = context.Events.First();
            entity.Subscribe(user);
            context.SaveChanges();
            return entity;
        });
        for (var i = 0; i < n; i++)
        {
            var request = BuildUnsubscribeRequest(entity.Id);

            var response = await client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var model = response.ReadContentAs<SubscriptionsCountViewModel>();
            model.EventId.Should().Be(entity.Id);
            model.Count.Should().Be(0);
        }
    }

    [Test]
    public async Task Response200_WhenWasNotSubscribed()
    {
        var (_, client) = await CreateAuthorizedClient();
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });
        var request = BuildUnsubscribeRequest(entity.Id);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var model = response.ReadContentAs<SubscriptionsCountViewModel>();
        model.EventId.Should().Be(entity.Id);
        model.Count.Should().Be(0);
    }

    private HttpRequestMessage BuildUnsubscribeRequest(Guid entityId)
        => new(HttpMethod.Delete, BuildUnsubscribeQuery(entityId));
}