using Eventnet.DataAccess.Configurations;
using Eventnet.DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// Context auto init own properties
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace Eventnet.DataAccess;

public class ApplicationDbContext : IdentityDbContext<UserEntity>
{
    public DbSet<EventEntity> Events { get; set; }
    public DbSet<TagEntity> Tags { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new EventConfiguration());
        builder.ApplyConfiguration(new SubscriptionEntityConfiguration());
        builder.ApplyConfiguration(new UserRolesConfiguration());
        builder.ApplyConfiguration(new TagEntityConfiguration());
    }
}