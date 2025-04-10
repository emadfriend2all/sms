using Finbuckle.MultiTenant;
using Showmatics.Application.Common.Exceptions;
using Showmatics.Application.Common.Persistence;
using Showmatics.Application.Multitenancy;
using Showmatics.Infrastructure.Persistence;
using Showmatics.Infrastructure.Persistence.Initialization;
using Mapster;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace Showmatics.Infrastructure.Multitenancy;

internal class TenantService : ITenantService
{
    private readonly IMultiTenantStore<FSHTenantInfo> _tenantStore;
    private readonly IConnectionStringSecurer _csSecurer;
    private readonly IDatabaseInitializer _dbInitializer;
    private readonly IStringLocalizer _t;
    private readonly ITenantInfo _tenantInfo;
    private readonly DatabaseSettings _dbSettings;
    private readonly TenantDbContext _tenantDbContext;

    public TenantService(
        IMultiTenantStore<FSHTenantInfo> tenantStore,
        IConnectionStringSecurer csSecurer,
        IDatabaseInitializer dbInitializer,
        IStringLocalizer<TenantService> localizer,
        IOptions<DatabaseSettings> dbSettings,
        ITenantInfo tenantInfo,
        TenantDbContext tenantDbContext)
    {
        _tenantStore = tenantStore;
        _csSecurer = csSecurer;
        _dbInitializer = dbInitializer;
        _t = localizer;
        _dbSettings = dbSettings.Value;
        _tenantInfo = tenantInfo;
        _tenantDbContext = tenantDbContext;
    }

    public async Task<List<TenantDto>> GetAllAsync()
    {
        var tenants = (await _tenantStore.GetAllAsync()).Adapt<List<TenantDto>>();
        foreach (var tenant in tenants)
        {
            tenant.TenantModules = (await GetTenantModulesAsync(tenant.Id))?.Select(x => x.AppModule.Name).ToList();
        }

        tenants.ForEach(t => t.ConnectionString = _csSecurer.MakeSecure(t.ConnectionString));
        return tenants;
    }

    public async Task<bool> ExistsWithIdAsync(string id) =>
        await _tenantStore.TryGetAsync(id) is not null;

    public async Task<bool> ExistsWithNameAsync(string name) =>
        (await _tenantStore.GetAllAsync()).Any(t => t.Name == name);

    public async Task<TenantDto> GetByIdAsync(string id)
    {
        var tenant = await GetTenantInfoAsync(id);
        var tenantDto = tenant.Adapt<TenantDto>();
        tenantDto.TenantModules = tenant.Modules?.Select(x => x.AppModule.Name).ToList();
        return tenantDto;
    }

    public async Task<TenantDto> GetCurrentsync()
    {
        string currentTenantId = _tenantInfo.Id ?? string.Empty;
        var fSHTenant = await GetTenantInfoAsync(currentTenantId!);
        var tenantDto = fSHTenant.Adapt<TenantDto>();
        tenantDto.TenantModules = fSHTenant.Modules?.Select(x => x.AppModule.Name).ToList();
        return tenantDto;
    }

    public async Task<string> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken)
    {
        if (request.ConnectionString?.Trim() == _dbSettings.ConnectionString.Trim()) request.ConnectionString = string.Empty;

        var tenant = new FSHTenantInfo(
            request.Id,
            request.Name,
            request.ConnectionString,
            request.AdminEmail,
            request.Issuer,
            request.BankAccount,
            request.Address,
            request.PhoneNumber,
            request.LogoUrl,
            request.PrimaryColor,
            request.SecondaryColor,
            request.HeaderUrl,
            request.FooterUrl,
            request.BankName,
            request.OwnerName,
            request.Title,
            request.RegistrationNo);

        var appModules = await GetAppModulesAsync();
        foreach (string module in request.TenantModules)
        {
            var appModule = appModules?.FirstOrDefault(x => x.Name == module);
            tenant.Modules.Add(new TenantModule
            {
                TenantId = tenant.Id,
                AppModuleId = appModule?.Id ?? 0,
                IsActive = true,
                AppModule = appModule ?? new()
            });
        }

        await _tenantStore.TryAddAsync(tenant);

        // TODO: run this in a hangfire job? will then have to send mail when it's ready or not
        try
        {
            await _dbInitializer.InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
        }
        catch
        {
            await _tenantStore.TryRemoveAsync(request.Id);
            throw;
        }

        return tenant.Id;
    }

    public async Task<string> UpdateAsync(UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        var tenant = await GetTenantInfoAsync(request.Id);
        request.Adapt(tenant);

        await RemoveCurrentTenantModulesAsync(tenant.Id);
        var appModules = await GetAppModulesAsync();
        foreach (string module in request.TenantModules)
        {
            var appModule = appModules?.FirstOrDefault(x => x.Name == module);
            tenant.Modules.Add(new TenantModule
            {
                TenantId = tenant.Id,
                AppModuleId = appModule?.Id ?? 0,
                IsActive = true,
                AppModule = appModule ?? new()
            });
        }

        await _tenantStore.TryUpdateAsync(tenant);
        return _t["Tenant {0} is updated.", request.Id];
    }

    public async Task<string> ActivateAsync(string id)
    {
        var tenant = await GetTenantInfoAsync(id);

        if (tenant.IsActive)
        {
            throw new ConflictException(_t["Tenant is already Activated."]);
        }

        tenant.Activate();

        await _tenantStore.TryUpdateAsync(tenant);

        return _t["Tenant {0} is now Activated.", id];
    }

    public async Task<string> DeactivateAsync(string id)
    {
        var tenant = await GetTenantInfoAsync(id);
        if (!tenant.IsActive)
        {
            throw new ConflictException(_t["Tenant is already Deactivated."]);
        }

        tenant.Deactivate();
        await _tenantStore.TryUpdateAsync(tenant);
        return _t["Tenant {0} is now Deactivated.", id];
    }

    public async Task<string> UpdateSubscription(string id, DateTime extendedExpiryDate)
    {
        var tenant = await GetTenantInfoAsync(id);
        tenant.SetValidity(extendedExpiryDate);
        return _t["Tenant {0}'s Subscription Upgraded. Now Valid till {1}.", id, tenant.ValidUpto];
    }

    public async Task<List<AppModule>> GetAppModulesAsync()
    {
        var modules = await _tenantDbContext.AppModules.ToListAsync();
        return modules;
    }

    public async Task RemoveCurrentTenantModulesAsync(string tenantId)
    {
        var modules = await _tenantDbContext.TenantModules.Where(x => x.TenantId == tenantId).ToListAsync();
        _tenantDbContext.TenantModules.RemoveRange(modules);
    }
    public async Task<List<TenantModule>> GetTenantModulesAsync(string id)
    {
        var modules = await _tenantDbContext.TenantModules.Include(x => x.AppModule)
            .Where(x => x.TenantId == id && x.IsActive && (x.ExpiryDate == null || x.ExpiryDate > DateTime.UtcNow)).ToListAsync();
        return modules;
    }

    private async Task<FSHTenantInfo> GetTenantInfoAsync(string id)
    {
        var tenant = await _tenantStore.TryGetAsync(id)
            ?? throw new NotFoundException(_t["{0} {1} Not Found.", typeof(FSHTenantInfo).Name, id]);
        tenant.Modules = await GetTenantModulesAsync(id);
        return tenant;
    }
}