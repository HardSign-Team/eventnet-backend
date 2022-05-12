using Eventnet.DataAccess.Entities;

namespace Eventnet.DataAccess.Extensions;

public static class PhotoEntityExtensions
{
    public static IQueryable<PhotoEntity> ForEvent(this IQueryable<PhotoEntity> query, Guid eventId)
    {
        return query.Where(x => x.EventId == eventId);
    }
}