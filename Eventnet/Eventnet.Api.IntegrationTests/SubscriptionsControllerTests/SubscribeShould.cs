using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Subscriptions;
using Eventnet.Api.UnitTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.SubscriptionsControllerTests;

public class SubscribeShould : SubscriptionsApiTestsBase
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
        var request = BuildSubscribeRequest(entity.Id);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Response404_WhenEventNotFound()
    {
        var (_, client) = await CreateAuthorizedClient();
        var request = BuildSubscribeRequest(Guid.Empty);

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
        var request = BuildSubscribeRequest(entity.Id);

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
        var request = BuildSubscribeRequest(entity.Id);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(10)]
    public async Task Response200_WhenSubscribeNTimes(int n)
    {
        var (_, client) = await CreateAuthorizedClient();
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });
        for (var i = 0; i < n; i++)
        {
            var request = BuildSubscribeRequest(entity.Id);

            var response = await client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var model = response.ReadContentAs<SubscriptionsCountViewModel>();
            model.EventId.Should().Be(entity.Id);
            model.Count.Should().Be(1);
        }
    }

    [Test]
    public async Task Response200_AndUpdateLastSubscriptionDate_WhenSubscribeSeveralTimes()
    {
        var (_, client) = await CreateAuthorizedClient();
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });

        await client.SendAsync(BuildSubscribeRequest(entity.Id));
        var subscription1 = ApplyToDb(context => context.Subscriptions.First(x => x.EventId == entity.Id));
        Thread.Sleep(500);
        await client.SendAsync(BuildSubscribeRequest(entity.Id));
        var subscription2 = ApplyToDb(context => context.Subscriptions.First(x => x.EventId == entity.Id));

        subscription2.SubscriptionDate.Should().BeAfter(subscription1.SubscriptionDate);
    }

    private HttpRequestMessage BuildSubscribeRequest(Guid entityId)
        => new(HttpMethod.Put, BuildSubscribeQuery(entityId));
}