using Eventnet.DataAccess.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
// Context auto init own properties
#pragma warning disable CS8618

namespace Eventnet.DataAccess;

public class ApplicationDbContext : IdentityDbContext<UserEntity>
{
    public DbSet<EventEntity> Events { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new EventConfiguration());
    }
}