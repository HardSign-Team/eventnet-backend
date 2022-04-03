using Eventnet.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventnet.DataAccess.Configurations;

public class UserRolesConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole(UserRoles.Admin)
                { NormalizedName = UserRoles.Admin.ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() },
            new IdentityRole(UserRoles.User)
                { NormalizedName = UserRoles.User.ToUpper(), ConcurrencyStamp = Guid.NewGuid().ToString() });
    }
}