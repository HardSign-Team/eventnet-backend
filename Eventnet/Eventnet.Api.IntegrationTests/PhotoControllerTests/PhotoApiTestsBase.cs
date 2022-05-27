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

    protected Uri BuildGetTitlePhotos() => BuildGetTitlePhotosUri();

    protected Uri BuildGetTitlePhotosUri()
    {
        var builder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/title",
        };
        return builder.Uri;
    }
}