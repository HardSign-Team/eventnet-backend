using System;
using System.Web;
using Eventnet.Api.Tests.Helpers;

namespace Eventnet.Api.Tests.EventControllerTests;

public abstract class EventApiTestsBase : TestsBase
{
    private const string BaseRoute = "/api/events";

    protected Uri BuildEventsByIdUri(Guid guid) => BuildEventsByIdUri(guid.ToString());

    protected Uri BuildEventsByIdUri(string guid)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/{HttpUtility.UrlEncode(guid)}"
        };
        return uriBuilder.Uri;
    }
}