using Eventnet.DataAccess;
using Eventnet.DataAccess.Entities;
using Eventnet.Infrastructure.PhotoServices;

namespace Eventnet.Api.Services.UserAvatars;

public class UserAvatarsService : IUserAvatarsService
{
    private readonly IPhotoStorageService photoStorageService;
    private readonly ApplicationDbContext dbContext;
    
    public UserAvatarsService(
        IPhotoStorageService photoStorageService,
        ApplicationDbContext dbContext)
    {
        this.photoStorageService = photoStorageService;
        this.dbContext = dbContext;
    }
    
    public async Task<Guid> UploadAvatarAsync(UserEntity user, IFormFile avatar)
    {
        if(user.AvatarId.HasValue)
            await DeleteUserAvatarAsync(user.AvatarId.Value);
        
        var avatarId = Guid.NewGuid();

        await SaveAvatarAsync(avatar, avatarId);

        dbContext.Attach(user);
        user.AvatarId = avatarId;
        await dbContext.SaveChangesAsync();

        return avatarId;
    }

    private async Task SaveAvatarAsync(IFormFile avatar, Guid avatarId)
    {
        await using var memoryStream = new MemoryStream();
        await avatar.CopyToAsync(memoryStream);
        var photo = new Domain.Photo(memoryStream.ToArray(), avatar.ContentType);
        photoStorageService.Save(photo, avatarId);
    }

    private Task DeleteUserAvatarAsync(Guid avatarId)
    {
        var path = Directory.GetFiles("static", avatarId + ".*").FirstOrDefault();

        return path is null
            ? Task.CompletedTask
            : Task.Run(() => File.Delete(path));
    }
}
