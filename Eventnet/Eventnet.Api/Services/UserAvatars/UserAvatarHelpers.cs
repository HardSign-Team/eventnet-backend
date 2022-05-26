namespace Eventnet.Api.Services.UserAvatars;

public static class UserAvatarHelpers
{
    private const string DefaultAvatar = "default-avatar.jpeg";
    public static string GetUserAvatar(Guid? avatarId)
    {
        if (!avatarId.HasValue)
            return DefaultAvatar;

        var path = Directory.GetFiles("static", avatarId + ".*").FirstOrDefault();

        return path == null
            ? DefaultAvatar
            : Path.GetFileName(path);
    }
}