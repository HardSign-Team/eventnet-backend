using Eventnet.DataAccess.Models;
using Microsoft.AspNetCore.Identity;

namespace Eventnet.DataAccess.Entities;

public class UserEntity : IdentityUser<Guid>
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Gender Gender { get; set; }

    public DateTime BirthDate { get; set; }

    public Guid? AvatarId { get; set; }
}