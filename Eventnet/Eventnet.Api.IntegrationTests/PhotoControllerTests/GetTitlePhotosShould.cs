using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using Eventnet.Api.Models.Photos;
using Eventnet.Api.UnitTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.PhotoControllerTests;

[TestFixture]
public class GetTitlePhotosShould : PhotoApiTestsBase
{
    [Test]
    public async Task Response200_WhenCorrectModel()
    {
        FillDatabase();
        var eventIds = ApplyToDb(context => context.Events.Take(3).Select(ev => ev.Id).ToArray());

        var response = await PostAsync(eventIds);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var viewModels = response.ReadContentAs<List<PhotoViewModel>>();
        viewModels.Should().HaveCount(eventIds.Length);
        viewModels.Select(x => x.Url).Should().NotContain(x => string.IsNullOrEmpty(x));
    }

    private void FillDatabase()
    {
        ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users.ToList());
            context.AddPhotos(context.Events.ToList());
        });
    }

    private async Task<HttpResponseMessage> PostAsync(Guid[] guids)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, BuildGetTitlePhotos());

        return await HttpClient.PostAsJsonAsync(request.RequestUri, new EventIdsListModel(guids));
    }
}