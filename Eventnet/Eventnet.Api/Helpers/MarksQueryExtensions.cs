using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.Helpers;

public static class MarksQueryExtensions
{
    public static IQueryable<MarkEntity> Of(this IQueryable<MarkEntity> query, UserEntity userEntity)
    {
        return query.Where(x => x.UserId == userEntity.Id);
    }   
    
    public static IQueryable<MarkEntity> For(this IQueryable<MarkEntity> query, Guid evenId)
    {
        return query.Where(x => x.EventId == evenId);
    }
    
    public static IQueryable<MarkEntity> Likes(this IQueryable<MarkEntity> query)
    {
        return query.Where(x=> x.IsLike);
    }
    
    public static IQueryable<MarkEntity> Dislikes(this IQueryable<MarkEntity> query)
    {
        return query.Where(x=> !x.IsLike);
    }
}