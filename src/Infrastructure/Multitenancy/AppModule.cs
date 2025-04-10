using Showmatics.Domain.Common.Contracts;

namespace Showmatics.Infrastructure.Multitenancy;

public class AppModule : BaseEntity<int>
{
    public string Name { get; set; } = default!;
    public bool Description { get; set; }
}