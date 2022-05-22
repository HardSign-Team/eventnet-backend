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
    
    public async Task<string> UploadAvatarAsync(UserEntity user, IFormFile avatar)
    {
        if(user.AvatarId.HasValue)
            await DeleteUserAvatarAsync(user.AvatarId.Value);
        
        var avatarId = Guid.NewGuid();

        var photo = await SaveAvatarAsync(avatar, avatarId);

        dbContext.Attach(user);
        user.AvatarId = avatarId;
        await dbContext.SaveChangesAsync();
                    
        return $"{avatarId.ToString()}{photo.Extension}";
    }

    private async Task<Domain.Photo> SaveAvatarAsync(IFormFile avatar, Guid avatarId)
    {
        await using var memoryStream = new MemoryStream();
        await avatar.CopyToAsync(memoryStream);
        var photo = new Domain.Photo(memoryStream.ToArray(), avatar.ContentType);
        photoStorageService.Save(photo, avatarId);

        return photo;
    }

    private Task DeleteUserAvatarAsync(Guid avatarId)
    {
        var avatarPath = photoStorageService.GetPhotoPath(avatarId) + ".jpeg";
        return Task.Run(() => File.Delete(avatarPath));
    }
}
