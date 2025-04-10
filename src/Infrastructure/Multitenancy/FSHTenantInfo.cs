using Finbuckle.MultiTenant;
using ShowMatic.Server.Domain.Catalog;
using Showmatics.Shared.Multitenancy;

namespace Showmatics.Infrastructure.Multitenancy;

public class FSHTenantInfo : ITenantInfo
{
    public FSHTenantInfo()
    {
    }

    public FSHTenantInfo(
        string id,
        string name,
        string? connectionString,
        string adminEmail,
        string? issuer = null,
        string? bankAccount = null,
        string? address = null,
        string? phoneNumber = null,
        string? logoUrl = null,
        string? primaryColor = null,
        string? secondaryColor = null,
        string? headeerUrl = null,
        string? footerUrl = null,
        string? bankName = null,
        string? ownerName = null,
        string? title = null,
        string? registrationNo = null,
        List<TenantModule>? modules = null)
    {
        Id = id;
        Identifier = id;
        Name = name;
        ConnectionString = connectionString ?? string.Empty;
        AdminEmail = adminEmail;
        IsActive = true;
        Issuer = issuer;
        BankAccount = bankAccount;
        BankName = bankName;
        Address = address;
        PhoneNumber = phoneNumber;
        HeaderUrl = headeerUrl;
        FooterUrl = footerUrl;

        // Add Default 1 Month Validity for all new tenants. Something like a DEMO period for tenants.
        ValidUpto = DateTime.UtcNow.AddMonths(1);
        LogoUrl = logoUrl;
        PrimaryColor = primaryColor;
        SecondaryColor = secondaryColor;
        OwnerName = ownerName;
        Title = title;
        RegistrationNo = registrationNo;
        Modules = modules ?? new List<TenantModule>();
    }

    /// <summary>
    /// The actual TenantId, which is also used in the TenantId shadow property on the multitenant entities.
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// The identifier that is used in headers/routes/querystrings. This is set to the same as Id to avoid confusion.
    /// </summary>
    public string Identifier { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string ConnectionString { get; set; } = default!;
    public string? OwnerName { get; set; }
    public string? Title { get; set; }
    public string? RegistrationNo { get; set; }
    public string AdminEmail { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public DateTime ValidUpto { get; private set; }
    public string? BankAccount { get; set; }
    public string? BankName { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? LogoUrl { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? HeaderUrl { get; set; }
    public string? FooterUrl { get; set; }
    public List<TenantModule> Modules { get; set; }
    /// <summary>
    /// Used by AzureAd Authorization to store the AzureAd Tenant Issuer to map against.
    /// </summary>
    public string? Issuer { get; set; }

    public void AddValidity(int months) =>
        ValidUpto = ValidUpto.AddMonths(months);

    public void SetValidity(in DateTime validTill) =>
        ValidUpto = ValidUpto < validTill
            ? validTill
            : throw new Exception("Subscription cannot be backdated.");

    public void Activate()
    {
        if (Id == MultitenancyConstants.Root.Id)
        {
            throw new InvalidOperationException("Invalid Tenant");
        }

        IsActive = true;
    }

    public void Deactivate()
    {
        if (Id == MultitenancyConstants.Root.Id)
        {
            throw new InvalidOperationException("Invalid Tenant");
        }

        IsActive = false;
    }

    string? ITenantInfo.Id { get => Id; set => Id = value ?? throw new InvalidOperationException("Id can't be null."); }
    string? ITenantInfo.Identifier { get => Identifier; set => Identifier = value ?? throw new InvalidOperationException("Identifier can't be null."); }
    string? ITenantInfo.Name { get => Name; set => Name = value ?? throw new InvalidOperationException("Name can't be null."); }
    string? ITenantInfo.ConnectionString { get => ConnectionString; set => ConnectionString = value ?? throw new InvalidOperationException("ConnectionString can't be null."); }
}