using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventnet.DataAccess.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<PhotoEntity>
{
    public void Configure(EntityTypeBuilder<PhotoEntity> builder)
    {
        builder.Property(x => x.Id);
        builder.HasOne<EventEntity>()
            .WithMany()
            .HasForeignKey(x => x.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}