using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.Services.UserAvatars;

public interface IUserAvatarsService
{
    Task<Guid> UploadAvatarAsync(UserEntity user, IFormFile avatar);
}