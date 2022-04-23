using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Eventnet.Api.IntegrationTests.Helpers;
using Eventnet.Api.Models.Events;
using Eventnet.Api.Models.Filtering;
using Eventnet.Controllers;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Eventnet.Api.IntegrationTests.EventControllerTests;

public class GetEventsShould : EventApiTestsBase
{
    public const int DefaultPageSize = EventController.DefaultPageSize;
    public const int MaxPageSize = EventController.MaxPageSize;

    [SetUp]
    public void Setup()
    {
        ApplyToDb(Utilities.ReinitializeDbForTests);
    }

    [TestCaseSource(nameof(GetIncorrectFilterRequests))]
    public async Task ResponseCode400_WhenIncorrectFilterParameters(EventsFilterModel eventsFilterModel)
    {
        var request = CreateDefaultRequestMessage(eventsFilterModel);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestCase(null)]
    [TestCase(-1)]
    [TestCase(0)]
    public async Task ResponseCode200_AndUseDefaultPageNumber_WhenIncorrect(int? pageNumber)
    {
        var request = CreateDefaultRequestMessage(pageNumber, DefaultPageSize);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagination = GetPaginationFromResponse(response);
        pagination.CurrentPage.Should().Be(1);
        pagination.PageSize.Should().BeGreaterOrEqualTo(DefaultPageSize);
    }

    [TestCase(null)]
    public async Task ResponseCode200_AndUseDefaultPageSize_WhenIncorrect(int? pageSize)
    {
        var request = CreateDefaultRequestMessage(1, pageSize);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagination = GetPaginationFromResponse(response);
        pagination.CurrentPage.Should().Be(1);
        pagination.PageSize.Should().Be(DefaultPageSize);
    }

    [TestCase(-1)]
    [TestCase(0)]
    public async Task ResponseCode200_AndUseMinPageSize_WhenIncorrect(int? pageSize)
    {
        var request = CreateDefaultRequestMessage(1, pageSize);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagination = GetPaginationFromResponse(response);
        pagination.CurrentPage.Should().Be(1);
        pagination.PageSize.Should().Be(1);
    }

    [Test]
    public async Task ResponseCode200_AndTrimPageSize_WhenGreaterThanMax()
    {
        var request = CreateDefaultRequestMessage(1, MaxPageSize + 1);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pagination = GetPaginationFromResponse(response);
        pagination.CurrentPage.Should().Be(1);
        pagination.PageSize.Should().BeGreaterOrEqualTo(MaxPageSize);
    }

    [TestCaseSource(nameof(GetInconsistentFilterRequests))]
    public async Task ResponseCode422_WhenInconsistentFilterParameters(EventsFilterModel eventsFilterModel)
    {
        var request = CreateDefaultRequestMessage(eventsFilterModel);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
    }

    [Test]
    public async Task ResponseCode200_WhenNoEvents()
    {
        var filterModel = new EventsFilterModel
        {
            RadiusLocation = new LocationFilterModel(new Location(0, 0), 5)
        };
        var events = new[]
        {
            GenerateEventAt(new Location(45, 0))
        };
        SetEvents(events);
        var request = CreateDefaultRequestMessage(filterModel, 1, 3);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
        var pagination = GetPaginationFromResponse(response);
        pagination.CurrentPage.Should().Be(1);
        pagination.PageSize.Should().Be(3);
        pagination.NextPageLink.Should().BeNull();
        pagination.PreviousPageLink.Should().BeNull();
        pagination.TotalCount.Should().Be(0);
        pagination.TotalPages.Should().Be(0);
    }

    [Test]
    public async Task ResponseCode200_WhenHasEvents_FirstPage()
    {
        var requestModel = new EventsFilterModel
        {
            RadiusLocation = new LocationFilterModel(new Location(0, 0), 500)
        };
        var events = new[]
        {
            GenerateEventAt(new Location(0.0013, 0)),
            GenerateEventAt(new Location(0.001, 0.0011)),
            GenerateEventAt(new Location(0.001, 0.0012)),
            GenerateEventAt(new Location(0.001, 0.0013)),
            GenerateEventAt(new Location(0.001, 0.0014)),
            GenerateEventAt(new Location(0.001, 0)),
            GenerateEventAt(new Location(45, 0))
        };
        SetEvents(events);
        var request = CreateDefaultRequestMessage(requestModel, 1, 3);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
        var pagination = GetPaginationFromResponse(response);
        pagination.CurrentPage.Should().Be(1);
        pagination.PageSize.Should().Be(3);
        pagination.NextPageLink.Should().NotBeNull();
        pagination.PreviousPageLink.Should().BeNull();
        pagination.TotalCount.Should().Be(6);
        pagination.TotalPages.Should().Be(2);
        var resultEvents = await response.Content.ReadFromJsonAsync<EventLocationModel[]>() ?? throw new Exception();
        resultEvents.Should().NotBeEmpty();
        resultEvents.Should().HaveCount(3);
    }

    [Test]
    public async Task ResponseCode200_WhenHasEvents_SecondPage()
    {
        var requestModel = new EventsFilterModel
        {
            RadiusLocation = new LocationFilterModel(new Location(0, 0), 500)
        };
        var events = new[]
        {
            GenerateEventAt(new Location(0.0013, 0)),
            GenerateEventAt(new Location(0.001, 0.0011)),
            GenerateEventAt(new Location(0.001, 0.0012)),
            GenerateEventAt(new Location(0.001, 0.0013)),
            GenerateEventAt(new Location(0.001, 0.0014)),
            GenerateEventAt(new Location(0.001, 0)),
            GenerateEventAt(new Location(45, 0))
        };
        SetEvents(events);
        var request = CreateDefaultRequestMessage(requestModel, 2, 3);

        var response = await HttpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
        var pagination = GetPaginationFromResponse(response);
        pagination.CurrentPage.Should().Be(2);
        pagination.PageSize.Should().Be(3);
        pagination.TotalCount.Should().Be(6);
        pagination.TotalPages.Should().Be(2);
        pagination.NextPageLink.Should().BeNull();
        pagination.PreviousPageLink.Should().Be(CreateDefaultRequestMessage(requestModel, 1, 3).RequestUri!.ToString());
        var resultEvents = await response.Content.ReadFromJsonAsync<EventLocationModel[]>() ?? throw new Exception();
        resultEvents.Should().NotBeEmpty();
        resultEvents.Should().HaveCount(3);
    }

    public static IEnumerable<TestCaseData> GetIncorrectFilterRequests()
    {
        yield return new TestCaseData(null)
            .SetName("FilterModel is null");
    }

    public static IEnumerable<TestCaseData> GetInconsistentFilterRequests()
    {
        yield return new TestCaseData(new EventsFilterModel
            {
#pragma warning disable CS8625
                RadiusLocation = new LocationFilterModel(null, 1)
#pragma warning restore CS8625
            })
            .SetName("Location is null");

        yield return new TestCaseData(new EventsFilterModel
            {
                RadiusLocation = new LocationFilterModel(new Location(0, 0), -1)
            })
            .SetName("Radius is negative");

        yield return new TestCaseData(new EventsFilterModel
            {
                RadiusLocation = new LocationFilterModel(new Location(0, 0), 0)
            })
            .SetName("Radius is zero");
    }

    private void SetEvents(IEnumerable<EventEntity> events)
    {
        ApplyToDb(context =>
        {
            context.Events.RemoveRange(context.Events);
            context.SaveChanges();
            context.Events.AddRange(events);
            context.SaveChanges();
        });
    }

    private EventEntity GenerateEventAt(Location location)
    {
        return new EventEntity(Guid.NewGuid(),
            "",
            DateTime.Today,
            DateTime.Today.AddDays(1),
            Guid.NewGuid().ToString(),
            "No description",
            new LocationEntity(location.Latitude, location.Longitude));
    }

    private HttpRequestMessage CreateDefaultRequestMessage(int? pageNumber, int? pageSize)
    {
        var filterModel = new EventsFilterModel
        {
            RadiusLocation = new LocationFilterModel(new Location(0, 0), 1)
        };
        return CreateDefaultRequestMessage(filterModel, pageNumber, pageSize);
    }

    private HttpRequestMessage CreateDefaultRequestMessage(
        EventsFilterModel eventsFilterModel,
        int? pageNumber = 1,
        int? pageSize = DefaultPageSize)
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Get;
        request.RequestUri = BuildEventsPageUri(eventsFilterModel, pageNumber, pageSize);
        request.Content = eventsFilterModel.SerializeToJsonContent();
        request.Headers.Add("Accept", "application/json");
        return request;
    }

    private Pagination GetPaginationFromResponse(HttpResponseMessage response)
    {
        var paginationHeader = response.GetRequiredHeader("X-Pagination").SingleOrDefault();
        var pagination = JsonConvert.DeserializeObject<Pagination>(paginationHeader);
        return pagination;
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    private class Pagination
    {
        public int? CurrentPage { get; set; }
        public string? NextPageLink { get; set; }
        public int? PageSize { get; set; }
        public string? PreviousPageLink { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
    }
}