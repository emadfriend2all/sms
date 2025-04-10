using Finbuckle.MultiTenant.Stores;
using Showmatics.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using ShowMatic.Server.Domain.Catalog;

namespace Showmatics.Infrastructure.Multitenancy;

public class TenantDbContext : EFCoreStoreDbContext<FSHTenantInfo>
{
    public TenantDbContext(DbContextOptions<TenantDbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<AppModule> AppModules => Set<AppModule>();
    public DbSet<TenantModule> TenantModules => Set<TenantModule>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FSHTenantInfo>().ToTable("Tenants", SchemaNames.MultiTenancy);
        modelBuilder.Entity<TenantModule>().ToTable("TenantModules", SchemaNames.MultiTenancy);
        modelBuilder.Entity<AppModule>().ToTable("AppModules", SchemaNames.MultiTenancy);
    }
}