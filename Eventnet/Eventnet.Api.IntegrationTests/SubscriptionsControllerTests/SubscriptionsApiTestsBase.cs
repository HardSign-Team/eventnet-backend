using System;
using System.Web;
using Eventnet.Api.IntegrationTests.Helpers;

namespace Eventnet.Api.IntegrationTests.SubscriptionsControllerTests;

public class SubscriptionsApiTestsBase : TestsBase
{
    private const string BaseRoute = "/api/subscriptions";

    protected Uri BuildCountOnEventUri(string? eventId)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/count/{HttpUtility.UrlEncode(eventId)}"
        };
        return uriBuilder.Uri;
    }
}