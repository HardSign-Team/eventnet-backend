using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Marks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.MarksControllerTests;

public class AddDislikeShould : MarksApiTestBase
{
    [TearDown]
    public void Teardown()
    {
        ApplyToDb(context =>
        {
            context.Marks.RemoveRange(context.Marks);
            context.SaveChanges();
        });
    }

    [Test]
    public async Task Response400_WhenNotAuthorized()
    {
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });
        var request = BuildAddDislikeRequest(entity.Id);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Response404_WhenEventNotFound()
    {
        var (_, client) = await CreateAuthorizedClient("TestUser", "TestPassword");
        var request = BuildAddDislikeRequest(Guid.Empty);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Response200_WhenAddSingleTime()
    {
        var (_, client) = await CreateAuthorizedClient("TestUser", "TestPassword");
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });
        var request = BuildAddDislikeRequest(entity.Id);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var viewModel = response.ReadContentAs<MarksCountViewModel>();
        viewModel.Likes.Should().Be(0);
        viewModel.Dislikes.Should().Be(1);
    }

    [TestCase(2)]
    [TestCase(10)]
    public async Task Response200_WhenAddSeveralTimes(int n)
    {
        var (_, client) = await CreateAuthorizedClient("TestUser", "TestPassword");
        var entity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return context.Events.First();
        });

        for (var i = 0; i < n; i++)
        {
            var request = BuildAddDislikeRequest(entity.Id);

            var response = await client.SendAsync(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var viewModel = response.ReadContentAs<MarksCountViewModel>();
            viewModel.Likes.Should().Be(0);
            viewModel.Dislikes.Should().Be(1);
        }
    }

    [Test]
    public async Task Response200_WhenLikeBefore()
    {
        var (user, client) = await CreateAuthorizedClient("TestUser", "TestPassword");
        var entity = await ApplyToDbAsync(async context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);
            return await context.Events.FirstAsync();
        });

        await ApplyToDbAsync(async context =>
        {
            var mark = entity.Like(user);
            context.Marks.Add(mark);
            await context.SaveChangesAsync();
        });

        var request = BuildAddDislikeRequest(entity.Id);
        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var viewModel = response.ReadContentAs<MarksCountViewModel>();
        viewModel.Likes.Should().Be(0);
        viewModel.Dislikes.Should().Be(1);
    }

    private HttpRequestMessage BuildAddDislikeRequest(Guid eventId)
        => new(HttpMethod.Post, BuildAddDislikeUri(eventId));
}