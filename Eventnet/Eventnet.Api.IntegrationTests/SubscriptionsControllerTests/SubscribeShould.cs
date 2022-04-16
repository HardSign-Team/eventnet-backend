using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Authentication;
using Eventnet.DataAccess.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
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
        var client = await CreateAuthorizedClient("TestUser", "TestPassword");
        var request = BuildSubscribeRequest(Guid.Empty);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Response409_WhenEventHasNotEndDate()
    {
        var client = await CreateAuthorizedClient("TestUser", "TestPassword");
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
        var client = await CreateAuthorizedClient("TestUser", "TestPassword");
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
        var client = await CreateAuthorizedClient("TestUser", "TestPassword");
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });
        var request = BuildSubscribeRequest(entity.Id);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveJsonContentEquivalentTo(new
        {
            eventId = entity.Id,
            count = 1
        });
    }

    [Test]
    public async Task Response200_AndUpdateLastSubscriptionDate_WhenSubscribeSeveralTimes()
    {
        var client = await CreateAuthorizedClient("TestUser", "TestPassword");
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });

        await client.SendAsync(BuildSubscribeRequest(entity.Id));
        var subscription1 = ApplyToDb(context => context.SubscriptionEntities.First(x => x.EventId == entity.Id));
        Thread.Sleep(500);
        await client.SendAsync(BuildSubscribeRequest(entity.Id));
        var subscription2 = ApplyToDb(context => context.SubscriptionEntities.First(x => x.EventId == entity.Id));

        subscription2.SubscriptionDate.Should().BeAfter(subscription1.SubscriptionDate);
    }

    private async Task<HttpClient> CreateAuthorizedClient(string username, string password)
    {
        var factory = GetScopeFactory();
        using var scope = factory.CreateScope();
        var userManager = scope.ServiceProvider.GetService<UserManager<UserEntity>>()!;
        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            var registerModel = new RegisterModel(username, $"{username}@test.com", password, null);
            await AuthorizationHelper.RegisterUserAsync(userManager, registerModel);
        }
        return await AuthorizationHelper.AuthorizeClient(HttpClient, username, password);
    }
    
    private HttpRequestMessage BuildSubscribeRequest(Guid entityId) => new(HttpMethod.Post, BuildSubscribeQuery(entityId));
}