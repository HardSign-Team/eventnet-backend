using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain;
using Eventnet.Domain.Events;
using Eventnet.Infrastructure.PhotoServices;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Infrastructure;

public class SaveToDbService : ISaveToDbService
{
    private readonly IMapper mapper;
    private readonly ApplicationDbContext dbContext;
    private readonly IPhotoToStorageSaveService storageSaveService;

    public SaveToDbService(
        IPhotoToStorageSaveService storageSaveService,
        IMapper mapper,
        ApplicationDbContext dbContext)
    {
        this.storageSaveService = storageSaveService;
        this.mapper = mapper;
        this.dbContext = dbContext;
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
        dbContext.Photos.Add(photoEntity);
        await dbContext.SaveChangesAsync();
    }
}