using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain;
using Eventnet.Domain.Events;
using Eventnet.Infrastructure.PhotoServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eventnet.Infrastructure;

public class SaveToDbService : ISaveToDbService
{
    private readonly IMapper mapper;
    private readonly IPhotoToStorageSaveService storageSaveService;
    private readonly IServiceScopeFactory factory;

    public SaveToDbService(IPhotoToStorageSaveService storageSaveService, IServiceScopeFactory factory, IMapper mapper)
    {
        this.storageSaveService = storageSaveService;
        this.factory = factory;
        this.mapper = mapper;
    }

    public async Task SavePhotosAsync(List<Photo> photos, Guid eventId)
    {
        foreach (var photo in photos)
        {
            var photoId = Guid.NewGuid();
            storageSaveService.Save(photo, photoId);
            var photoEntity = new PhotoEntity(photoId, eventId);
            await SavePhotoToDbAsync(photoEntity);
        }
    }

    public async Task SaveEventAsync(EventInfo info)
    {
        await using var dbContext = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var eventEntity = new EventEntity(info.EventId,
            info.OwnerId,
            info.StartDate,
            info.EndDate,
            info.Name,
            info.Description ?? "",
            mapper.Map<LocationEntity>(info.Location));
        dbContext.Events.Add(eventEntity);
        await SaveTagsAsync(eventEntity, info.Tags, dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SaveTagsAsync(EventEntity eventEntity, string[] tags, ApplicationDbContext dbContext)
    {
        var exists = await dbContext.Tags
            .Where(x => tags.Contains(x.Name))
            .ToDictionaryAsync(x => x.Name, x => x);

        var notExists = tags
            .Where(name => !exists.ContainsKey(name))
            .Select(name => new TagEntity(0, name))
            .ToList();
        await dbContext.Tags.AddRangeAsync(notExists);
        await dbContext.SaveChangesAsync();

        eventEntity.AddTags(exists.Values.Concat(notExists));
    }

    private async Task SavePhotoToDbAsync(PhotoEntity photoEntity)
    {
        await using var dbContext = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Photos.Add(photoEntity);
        await dbContext.SaveChangesAsync();
    }
}