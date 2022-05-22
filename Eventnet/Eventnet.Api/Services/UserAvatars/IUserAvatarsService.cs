using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.Services.UserAvatars;

public interface IUserAvatarsService
{
    Task<string> UploadAvatarAsync(UserEntity user, IFormFile avatar);
}