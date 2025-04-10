namespace Showmatics.Application.Multitenancy;

public class TenantModuleDto
{
    public string TenantId { get; set; } = default!;
    public int AppModuleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public AppModuleDto AppModule { get; set; } = default!;
}
