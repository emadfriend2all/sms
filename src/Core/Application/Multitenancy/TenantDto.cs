namespace Showmatics.Application.Multitenancy;

public class TenantDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ConnectionString { get; set; }
    public string AdminEmail { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime ValidUpto { get; set; }
    public string? Issuer { get; set; }
    public string? PhoneNumber { get; set; }
    public string? BankAccount { get; set; }
    public string? Address { get; set; }
    public string? Contact { get; set; }
    public string? OwnerName { get; set; }
    public string? Title { get; set; }
    public string? RegistrationNo { get; set; }
    public string? HeaderUrl { get; set; }
    public string? FooterUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? BankName { get; set; }
    public List<string>? TenantModules { get; set; }
}
