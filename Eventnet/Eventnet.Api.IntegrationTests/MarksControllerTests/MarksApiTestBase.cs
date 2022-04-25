using System;

namespace Eventnet.Api.IntegrationTests.MarksControllerTests;

public class MarksApiTestBase : TestsBase
{
    private const string BaseRoute = "/api/marks";

    protected Uri BuildAddLikeUri(Guid eventId)
    {
        var uri = new UriBuilder
        {
            Path = $"{BaseRoute}/likes/{eventId}"
        };
        return uri.Uri;
    }

    protected Uri BuildRemoveLikeUri(Guid eventId)
    {
        var uri = new UriBuilder
        {
            Path = $"{BaseRoute}/likes/{eventId}"
        };
        return uri.Uri;
    }

    protected Uri BuildAddDislikeUri(Guid eventId)
    {
        var uri = new UriBuilder
        {
            Path = $"{BaseRoute}/dislikes/{eventId}"
        };
        return uri.Uri;
    }

    protected Uri BuildRemoveDislikeUri(Guid eventId)
    {
        var uri = new UriBuilder
        {
            Path = $"{BaseRoute}/dislikes/{eventId}"
        };
        return uri.Uri;
    }
}