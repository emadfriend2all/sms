namespace Showmatics.Shared.Multitenancy;

public class TenantBasicInfoDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Contact { get; set; }
    public string? HeaderUrl { get; set; }
    public string? FooterUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public List<string>? Modules { get; set; }
}
