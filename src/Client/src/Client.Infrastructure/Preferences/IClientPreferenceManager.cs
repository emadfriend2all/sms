using MudBlazor;
using Showmatics.Shared.Multitenancy;

namespace Showmatics.Blazor.Client.Infrastructure.Preferences;

public interface IClientPreferenceManager : IPreferenceManager
{
    Task<MudTheme> GetCurrentThemeAsync();
    Task SetTenantColorsAsync(TenantBasicInfoDto? tenantInfo);
    Task<bool> ToggleDarkModeAsync();

    Task<bool> ToggleDrawerAsync();

    Task<bool> ToggleLayoutDirectionAsync();
}