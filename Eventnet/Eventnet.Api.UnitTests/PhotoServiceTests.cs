using System;
using System.Linq;
using System.Threading.Tasks;
using Eventnet.Api.Services.Photo;
using Eventnet.Api.UnitTests.Helpers;
using Eventnet.DataAccess.Entities;
using Eventnet.Infrastructure.PhotoServices;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Eventnet.Api.UnitTests;

[TestFixture]
public class PhotoServiceTests : BaseTests<PhotoServiceTests>
{
    private readonly IPhotoStorageService photoStorageService;

    [TearDown]
    public void TearDown()
    {
        using var context = CreateDbContext();
        Utilities.ReinitializeDbForTests(context);
    }

    public PhotoServiceTests()
    {
        var photoStorageServiceMock = new Mock<IPhotoStorageService>();
        photoStorageServiceMock.Setup(x => x.GetPhotoPath(It.IsAny<Guid>())).Returns<Guid>(arg => arg.ToString());
        photoStorageService = photoStorageServiceMock.Object;
    }

    [Test]
    public async Task GetTitlePhotos_ShouldReturnEmptyResult_WhenEmptyInput()
    {
        var sut = CreateSut();

        var actual = await sut.GetTitlePhotos("", Array.Empty<Guid>());

        actual.Should().HaveCount(0);
    }

    [Test]
    public async Task GetTitlePhotos_ShouldReturnPhotos_WhenEventsHavePhotos()
    {
        AddEvents();
        ApplyToDb(context => context.AddPhotos(context.Events.ToList()));
        var events = ApplyToDb(context => context.Events.Select(x => x.Id).ToArray());
        var sut = CreateSut();

        var actual = await sut.GetTitlePhotos("", events);

        actual.Should().HaveCount(events.Length);
        actual.Should().NotContainNulls();
    }

    [Test]
    public async Task GetTitlePhotos_ShouldReturnPhotos_WhenSomeEventsNotExist()
    {
        AddEvents();
        ApplyToDb(context => context.AddPhotos(context.Events.ToList()));
        var events = ApplyToDb(context => context.Events.Select(x => x.Id).ToList());
        var guids = events.Append(Guid.Empty).ToArray();
        var sut = CreateSut();

        var actual = await sut.GetTitlePhotos("", guids);

        actual.Should().HaveCount(events.Count);
        actual.Should().NotContainNulls();
    }

    [Test]
    public async Task GetTitlePhotos_ShouldReturnPhotos_WhenSomeEventsHaveNoPhotos()
    {
        const int havePhotosCount = 2;
        AddEvents();
        ApplyToDb(context => context.AddPhotos(context.Events.OrderBy(x => x.Id).Take(havePhotosCount).ToList()));
        var events = ApplyToDb(context => context.Events.Select(x => x.Id).ToArray());
        var sut = CreateSut();

        var actual = await sut.GetTitlePhotos("", events);

        actual.Should().HaveCount(havePhotosCount);
        actual.Should().NotContainNulls();
    }

    [Test]
    public async Task GetPhotoUrls_ShouldReturnEmptyArray_WhenEventNotExists()
    {
        var sut = CreateSut();

        var actual = await sut.GetPhotosViewModels("", Guid.Empty);

        actual.Should().HaveCount(0);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(10)]
    public async Task GetPhotoUrls_ShouldReturnAllPhotos_WhenEventHasPhotos(int photosCount)
    {
        AddEvents();
        var ev = ApplyToDb(context => context.Events.First());
        AddPhotos(ev, photosCount);
        var sut = CreateSut();

        var actual = await sut.GetPhotosViewModels("", ev.Id);

        actual.Should().HaveCount(photosCount);
        actual.Should().NotContainNulls();
    }

    private PhotoService CreateSut() => new(CreateDbContext(), photoStorageService);

    private void AddPhotos(EventEntity ev, int count)
    {
        ApplyToDb(context =>
        {
            var photos = Enumerable.Range(0, count).Select(_ => new PhotoEntity(Guid.NewGuid(), ev.Id));
            context.Photos.AddRange(photos);
            context.SaveChanges();
        });
    }

    private void AddEvents()
    {
        ApplyToDb(context =>
        {
            context.AddUsers();
            context.AddEvents(context.Users.ToList());
        });
    }
}