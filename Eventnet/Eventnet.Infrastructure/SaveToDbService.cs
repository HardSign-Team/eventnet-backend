using System.Drawing;
using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.Infrastructure.PhotoServices;
using Eventnet.Models;
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

    public async Task SavePhotos(List<Image> photos, Guid eventId)
    {
        foreach (var photo in photos)
        {
            var photoId = Guid.NewGuid();
            storageSaveService.Save(photo, photoId);
            var photoEntity = new PhotoEntity(photoId, eventId);
            await SavePhotoToDbAsync(photoEntity);
        }
    }

    private async Task SavePhotoToDbAsync(PhotoEntity photoEntity)
    {
        await using var dbContext = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Photos.Add(photoEntity);
        await dbContext.SaveChangesAsync();
    }

    public async Task SaveEvent(Event eventForSave)
    {
        await using var dbContext = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var eventEntity = mapper.Map<EventEntity>(eventForSave);
        dbContext.Events.Add(eventEntity);
        await dbContext.SaveChangesAsync();
    }
}