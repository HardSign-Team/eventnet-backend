using Eventnet.DataAccess.Entities;
using Eventnet.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventnet.DataAccess.Configurations;

public class UserRolesConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasData(new UserRole(UserRoles.Admin)
            {
                Id = Guid.NewGuid(),
                NormalizedName = UserRoles.Admin.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new UserRole(UserRoles.User)
            {
                Id = Guid.NewGuid(),
                NormalizedName = UserRoles.User.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
    }
}