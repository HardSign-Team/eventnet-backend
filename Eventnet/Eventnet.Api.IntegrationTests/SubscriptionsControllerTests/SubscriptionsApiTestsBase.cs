using System;
using System.Web;
using Eventnet.Api.IntegrationTests.Helpers;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.SubscriptionsControllerTests;

public class SubscriptionsApiTestsBase : TestsBase
{
    private const string BaseRoute = "/api/subscriptions";

    [TearDown]
    public void TearDown()
    {
        ApplyToDb(context => context.Clear());
    }

    protected Uri BuildCountOnEventUri(string? eventId)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/count/{HttpUtility.UrlEncode(eventId)}"
        };
        return uriBuilder.Uri;
    }

    protected Uri BuildSubscribeQuery(Guid eventId)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/subscribe/{HttpUtility.UrlEncode(eventId.ToString())}"
        };
        return uriBuilder.Uri;
    }

    protected Uri BuildUnsubscribeQuery(Guid eventId)
    {
        var uriBuilder = new UriBuilder(Configuration.BaseUrl)
        {
            Path = $"{BaseRoute}/unsubscribe/{HttpUtility.UrlEncode(eventId.ToString())}"
        };
        return uriBuilder.Uri;
    }
}