using Finbuckle.MultiTenant.EntityFrameworkCore;
using ShowMatic.Server.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ShowMatic.Server.Infrastructure.Persistence.Configuration.Catalog;

public class CustomerConfig : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.IsMultiTenant();
        builder.HasIndex(e => e.IdentityNumber).IsUnique();
        builder.HasIndex(e => e.RegistrationNo).IsUnique();
        builder.HasIndex(e => e.TaxNumber).IsUnique();
    }
}