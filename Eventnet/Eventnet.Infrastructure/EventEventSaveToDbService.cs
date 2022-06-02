using AutoMapper;
using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Eventnet.Infrastructure;

public class EventEventSaveToDbService : IEventSaveToDbService
{
    private readonly IMapper mapper;
    private readonly ApplicationDbContext dbContext;

    public EventEventSaveToDbService(
        IMapper mapper,
        ApplicationDbContext dbContext)
    {
        this.mapper = mapper;
        this.dbContext = dbContext;
    }

    public async Task SaveEventAsync(EventInfo info)
    {
        var eventEntity = new EventEntity(info.EventId,
            info.OwnerId,
            info.StartDate,
            info.EndDate,
            info.Name,
            info.Description,
            mapper.Map<LocationEntity>(info.Location));
        dbContext.Events.Add(eventEntity);
        await SaveTagsAsync(eventEntity, info.Tags, dbContext);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateEventAsync(EventInfo eventInfo)
    {
        var oldValue = await dbContext.Events.FindAsync(eventInfo.EventId);
        if (oldValue is not null)
        {
            oldValue.Description = eventInfo.Description;
            oldValue.Location = new LocationEntity(eventInfo.Location.Latitude, eventInfo.Location.Longitude);
            oldValue.Name = eventInfo.Name;
            oldValue.EndDate = eventInfo.EndDate;
            oldValue.StartDate = eventInfo.StartDate;
            await SaveTagsAsync(oldValue, eventInfo.Tags, dbContext);
            dbContext.Events.Update(oldValue);
            await dbContext.SaveChangesAsync();
        }
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
}