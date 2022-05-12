using System;
using System.Text;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace Eventnet.Api.IntegrationTests.PhotoControllerTests;

public class PhotoApiTestsBase : TestsBase
{
    private const string BaseRoute = "/api/photos";

    protected Uri BuildGetPhotosUri(string eventId) => new UriBuilder(Configuration.BaseUrl)
    {
        Path = $"{BaseRoute}/{eventId}"
    }.Uri;

    protected Uri BuildGetTitlePhotos(Guid[] eventIds) => BuildGetTitlePhotosUri(
        Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new EventIdsListModel(eventIds)))));

    protected Uri BuildGetTitlePhotosUri(string base64EventIds)
    {
        var qb = new QueryBuilder
        {
            { "events", base64EventIds }
        };
        var builder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/title",
            Query = qb.ToString()
        };
        return builder.Uri;
    }
}