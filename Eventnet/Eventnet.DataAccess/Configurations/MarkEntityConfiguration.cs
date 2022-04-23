using Eventnet.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventnet.DataAccess.Configurations;

public class MarkEntityConfiguration : IEntityTypeConfiguration<MarkEntity>
{
    public void Configure(EntityTypeBuilder<MarkEntity> builder)
    {
        builder.HasKey(x => new { x.EventId, x.UserId });
        builder.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<EventEntity>()
            .WithMany(x => x.Marks)
            .HasForeignKey(x => x.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.IsLike);
        builder.Property(x => x.Date);
    }
}