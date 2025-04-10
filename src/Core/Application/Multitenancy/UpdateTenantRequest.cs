namespace Showmatics.Application.Multitenancy;

public class UpdateTenantRequest : IRequest<string>
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? ConnectionString { get; set; }
    public string AdminEmail { get; set; } = default!;
    public string? Issuer { get; set; }
    public string? BankName { get; set; }
    public string? BankAccount { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? HeaderUrl { get; set; }
    public string? FooterUrl { get; set; }
    public string? OwnerName { get; set; }
    public string? Title { get; set; }
    public string? RegistrationNo { get; set; }
    public List<string> TenantModules { get; set; } = new();
}

public class UpdateTenantRequestHandler : IRequestHandler<UpdateTenantRequest, string>
{
    private readonly ITenantService _tenantService;

    public UpdateTenantRequestHandler(ITenantService tenantService) => _tenantService = tenantService;

    public Task<string> Handle(UpdateTenantRequest request, CancellationToken cancellationToken) =>
        _tenantService.UpdateAsync(request, cancellationToken);
}