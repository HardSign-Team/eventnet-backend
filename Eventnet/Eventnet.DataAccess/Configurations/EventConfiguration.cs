using Eventnet.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventnet.DataAccess.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.Property(x => x.Id);
        builder.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.OwnerId);
        builder.Property(x => x.StartDate);
        builder.Property(x => x.EndDate).IsRequired(false);
        builder.Property(x => x.Name);
        builder.Property(x => x.Description);
        builder.OwnsOne(x => x.Location);
    }
}