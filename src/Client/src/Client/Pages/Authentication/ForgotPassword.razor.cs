using Showmatics.Blazor.Client.Components.Common;
using Showmatics.Blazor.Client.Infrastructure.ApiClient;
using Showmatics.Blazor.Client.Shared;
using Showmatics.Shared.Multitenancy;
using Microsoft.AspNetCore.Components;
using Showmatics.Blazor.Client.Infrastructure.Preferences;

namespace Showmatics.Blazor.Client.Pages.Authentication;

public partial class ForgotPassword
{
    private readonly ForgotPasswordRequest _forgotPasswordRequest = new();
    private CustomValidation? _customValidation;
    private bool BusySubmitting { get; set; }
    private ClientPreference? _themePreference;
    [Inject]
    private IUsersClient UsersClient { get; set; } = default!;

    private string Tenant { get; set; } = MultitenancyConstants.Root.Id;
    private string _logoUrl = "Files/showmatics-logo.png";
    protected override async Task OnInitializedAsync()
    {
        _themePreference = await ClientPreferences.GetPreference() as ClientPreference;

        if (_themePreference == null) _themePreference = new ClientPreference();
        _logoUrl = _themePreference.LogoUrl;

    }

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.ForgotPasswordAsync(Tenant, _forgotPasswordRequest),
            Snackbar,
            _customValidation);

        BusySubmitting = false;
    }
}