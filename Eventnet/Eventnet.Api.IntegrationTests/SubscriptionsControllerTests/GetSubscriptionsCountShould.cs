using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Subscriptions;
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
        var (eventEntity, subscriptions) = ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users);

            var entity = context.Events.First();
            foreach (var user in context.Users)
            {
                entity.Subscribe(user);
            }

            var subscriptionEntities = context.Users.Select(user => entity.Subscribe(user)).ToArray();
            context.Subscriptions.AddRange(subscriptionEntities);
            context.SaveChanges();

            return (entity, subscriptionEntities);
        });
        var request = CreateCountRequest(eventEntity.Id.ToString());

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var viewModel = response.ReadContentAs<SubscriptionsCountViewModel>();
        viewModel.Should().Be(new SubscriptionsCountViewModel(eventEntity.Id, subscriptions.Length));
    }

    private HttpRequestMessage CreateCountRequest(string? eventId)
        => new(HttpMethod.Get, BuildCountOnEventUri(eventId));
}