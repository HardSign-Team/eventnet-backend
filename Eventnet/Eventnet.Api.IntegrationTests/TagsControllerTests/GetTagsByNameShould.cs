using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Tags;
using Eventnet.DataAccess.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.TagsControllerTests;

public class GetTagsByNameShould : TagsApiTestsBase
{
    [TestCase("")]
    [TestCase(null)]
    public async Task ResponseCode404_WhenIncorrectName(string name)
    {
        var request = CreateDefaultRequest(name, 10);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestCase(" ")]
    [TestCase("  ")]
    public async Task ResponseCode422_WhenEmptyName(string name)
    {
        var request = CreateDefaultRequest(name, 10);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async Task ResponseCode200_WhenCorrectName()
    {
        const int maxCount = 3;
        CreateTags(new[]
        {
            "Aa",
            "bbb",
            "aaaa",
            "aab",
            "aabb",
            "aabbb",
            "aabbbb",
            "aabbbbb",
            "aabbbbb"
        });
        var request = CreateDefaultRequest("a", maxCount);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tags = response.ReadContentAs<List<TagNameViewModel>>();
        tags.Should().HaveCount(maxCount);
    }

    private void CreateTags(string[] names)
    {
        ApplyToDb(c =>
        {
            c.Tags.AddRange(names.Select((name, idx) => new TagEntity(idx + 1, name)));
            c.SaveChanges();
        });
    }

    private HttpRequestMessage CreateDefaultRequest(string name, int maxCount)
    {
        var request = new HttpRequestMessage();
        request.RequestUri = BuildSearchByName(name, maxCount);
        request.Method = HttpMethod.Get;
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return request;
    }
}