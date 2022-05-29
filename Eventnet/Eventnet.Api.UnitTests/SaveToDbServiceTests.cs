using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events;
using Eventnet.Infrastructure;
using Eventnet.Infrastructure.PhotoServices;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Eventnet.Api.UnitTests;

public class SaveToDbServiceTests : BaseTests<SaveToDbServiceTests>
{
    [SetUp]
    public void Setup()
    {
        using var context = CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    [TestCase(new string[0], new string[0])]
    [TestCase(new[] { "tag1", "tag2", "tag3" }, new string[0])]
    [TestCase(new[] { "tag1", "tag2", "tag3" }, new[] { "tag1", "tag2", "tags4" })]
    public async Task SaveEventAsync_ShouldSaveTags_WhenHasTags(string[] tagsInModel, string[] tagsInDb)
    {
        await using var context = CreateDbContext();
        var sut = CreateSut(context);
        await SaveTags(tagsInDb, context);
        var info = CreateEventInfo() with { Tags = tagsInModel };

        await sut.SaveEventAsync(info);

        tagsInModel.OrderBy(x => x).Should().BeSubsetOf(context.Tags.Select(x => x.Name).OrderBy(x => x));
        var entity = await context.Events.Select(x => new { x.Id, x.Tags }).FirstAsync(x => x.Id == info.EventId);
        entity.Tags.Select(x => x.Name).ToHashSet().Should().Equal(tagsInModel.ToHashSet());
    }

    private IEventSaveToDbService CreateSut(ApplicationDbContext context) =>
        new EventEventSaveToDbService(Mock.Of<IPhotoStorageService>(), CreateMapper(), context);

    private static async Task SaveTags(IEnumerable<string> tagsInDb, ApplicationDbContext context)
    {
        await context.Tags.AddRangeAsync(tagsInDb.Select(x => new TagEntity(x)));
        await context.SaveChangesAsync();
    }

    private EventInfo CreateEventInfo() => new(Guid.NewGuid(),
        Guid.NewGuid(),
        DateTime.Now,
        DateTime.Now.AddDays(3),
        "Event",
        "Description",
        new Location(12, 32),
        Array.Empty<string>());
}