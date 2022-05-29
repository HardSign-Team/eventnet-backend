using Microsoft.AspNetCore.Identity;

namespace Eventnet.DataAccess.Entities;

public class UserRole : IdentityRole<Guid>
{
    // ReSharper disable once UnusedMember.Global
    public UserRole()
    {
    }

    public UserRole(string roleName) : base(roleName)
    {
    }
}