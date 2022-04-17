using Eventnet.DataAccess.Entities;
using Eventnet.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventnet.DataAccess.Configurations;

public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
            .Property(x => x.Gender)
            .HasConversion(v => v == Gender.Male,
                v => v ? Gender.Male : Gender.Female)
            .IsRequired();
    }
}