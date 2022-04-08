using Eventnet.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventnet.DataAccess.Configurations;

public class EventTagEntityConfiguration : IEntityTypeConfiguration<EventTagEntity>
{
    public void Configure(EntityTypeBuilder<EventTagEntity> builder)
    {
        builder
            .HasKey(x => new { x.EventId, x.TagId });
        builder.HasOne<EventEntity>()
            .WithMany()
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        builder.HasOne<TagEntity>()
            .WithMany()
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}