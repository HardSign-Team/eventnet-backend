using System;
using System.Net.Http;
using System.Web;
using Eventnet.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Api.Tests.EventControllerTests;

public abstract class EventApiTestsBase
{
    protected readonly HttpClient HttpClient = new();
    protected const string BaseRoute = "/api/events";

    protected Uri BuildEventsByIdUri(Guid guid)
    {
        return BuildEventsByIdUri(guid.ToString());
    }

    protected Uri BuildEventsByIdUri(string guid)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/{HttpUtility.UrlEncode(guid)}"
        };
        return uriBuilder.Uri;
    }

}