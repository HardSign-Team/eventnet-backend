using Eventnet.DataAccess.Models;

#pragma warning disable CS8618

namespace Eventnet.Api.Models;

public class UserViewModel
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public Gender Gender { get; init; }
    public DateTime BirthDate { get; init; }
    public string AvatarUrl { get; init; }

    public void Deconstruct(
        out Guid id,
        out string userName,
        out string email,
        out Gender gender,
        out DateTime birthDate,
        out string avatarUrl)
    {
        id = Id;
        userName = UserName;
        email = Email;
        gender = Gender;
        birthDate = BirthDate;
        avatarUrl = AvatarUrl;
    }
}