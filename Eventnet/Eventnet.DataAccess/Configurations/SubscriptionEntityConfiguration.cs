using Eventnet.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventnet.DataAccess.Configurations;

public class SubscriptionEntityConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionEntity> builder)
    {
        builder.HasKey(x => new { x.EventId, x.UserId });
        builder.Property(x => x.SubscriptionDate);
        builder.Property(x => x.UserId);
        builder.HasOne<EventEntity>()
            .WithMany(x => x.Subscriptions)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}