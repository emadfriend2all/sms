using Showmatics.Domain.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showmatics.Infrastructure.Multitenancy;
public class TenantModule : AuditableEntity<int>, IAggregateRoot
{
    public string TenantId { get; set; } = default!;
    public int AppModuleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public FSHTenantInfo Tenant { get; set; } = default!;
    public AppModule AppModule { get; set; } = default!;
}