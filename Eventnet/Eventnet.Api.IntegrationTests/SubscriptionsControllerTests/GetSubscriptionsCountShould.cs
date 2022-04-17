using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.SubscriptionsControllerTests;

public class GetSubscriptionsCountShould : SubscriptionsApiTestsBase
{
    [TestCase(null)]
    [TestCase(" ")]
    [TestCase("0000-0000-0000-0000")]
    public async Task Response404_WhenEventNotFound(string? eventId)
    {
        var request = CreateCountRequest(eventId);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Response200_WhenEventExists()
    {
        var eventEntity = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);

            var entity = context.Events.First();
            foreach (var user in context.Users)
            {
                entity.Subscribe(user);
            }

            context.SaveChanges();

            return entity;
        });
        var request = CreateCountRequest(eventEntity.Id.ToString());

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveJsonContentEquivalentTo(new
        {
            eventId = eventEntity.Id,
            count = eventEntity.Subscriptions.Count
        });
    }

    private HttpRequestMessage CreateCountRequest(string? eventId)
        => new(HttpMethod.Get, BuildCountOnEventUri(eventId));
}