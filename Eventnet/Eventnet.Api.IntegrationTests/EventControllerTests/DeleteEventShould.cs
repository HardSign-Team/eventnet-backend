using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.UnitTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.EventControllerTests;

public class DeleteEventShould : EventApiTestsBase
{
    [TestCaseSource(nameof(GetInvalidGuids))]
    public async Task ResponseCode405_WhenEventInvalidGuid(string? guid)
    {
        var (_, client) = await CreateAuthorizedClient();
        var request = CreateDefaultRequest(guid);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        response.ShouldNotHaveHeader("Content-Type");
    }

    [Test]
    public async Task ResponseCode404_WhenEventNotFound()
    {
        var (_, client) = await CreateAuthorizedClient();
        var request = CreateDefaultRequest(Guid.NewGuid());

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.ShouldNotHaveHeader("Content-Type");
    }

    [Test]
    public async Task ResponseCode400_WhenEmptyGuid()
    {
        var (_, client) = await CreateAuthorizedClient();
        var request = CreateDefaultRequest(Guid.Empty);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.ShouldNotHaveHeader("Content-Type");
    }

    [Test]
    public async Task ResponseCode200_WhenDeleteGuid()
    {
        var (_, client) = await CreateAuthorizedClient();
        var eventEntity = EventEntityMother.CreateEventEntity();
        ApplyToDb(context =>
        {
            context.Events.Add(eventEntity);
            context.SaveChanges();
        });
        var request = CreateDefaultRequest(eventEntity.Id);

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ReadContentAs<Guid>().Should().Be(eventEntity.Id);
        ApplyToDb(context => context.Events.ToArray()).Should().BeEmpty();
    }

    public static IEnumerable<TestCaseData> GetInvalidGuids()
    {
        yield return new TestCaseData(null).SetName("Guid is null");
        yield return new TestCaseData("").SetName("Empty string");
    }

    private HttpRequestMessage CreateDefaultRequest(string? guid)
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Delete;
        request.RequestUri = BuildDeleteEventUri(guid);
        return request;
    }

    private HttpRequestMessage CreateDefaultRequest(Guid guid) => CreateDefaultRequest(guid.ToString());
}