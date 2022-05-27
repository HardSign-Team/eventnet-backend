using Eventnet.DataAccess.Entities;

namespace Eventnet.Api.Services.UserAvatars;

public static class UserAvatarHelpers
{
    public static string GetUserAvatar(UserEntity user) =>
        !user.AvatarId.HasValue
            ? "default-avatar.jpeg"
            : $"{user.AvatarId.Value}.jpeg";
}