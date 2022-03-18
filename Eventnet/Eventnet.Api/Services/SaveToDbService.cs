using System.Drawing;
using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.Services.ImageServices;

namespace Eventnet.Services;

public class SaveToDbService : ISaveToDbService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IMapper mapper;
    private readonly IImageToDbPreparer preparer;

    public SaveToDbService(IImageToDbPreparer preparer, IServiceScopeFactory factory, IMapper mapper)
    {
        this.preparer = preparer;
        this.mapper = mapper;
        dbContext = factory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    public void SaveImages(List<Image> images, Guid id)
    {
        var path = preparer.Save(images, id);
        var photosEntity = new PhotosEntity(path, id);
        dbContext.Photos.Add(photosEntity);
    }

    public void SaveEvent(RabbitMqMessage message)
    {
        var eventEntity = mapper.Map<EventEntity>(message.Event);
        dbContext.Events.Add(eventEntity);
    }
}