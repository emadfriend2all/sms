using Showmatics.Blazor.Client.Infrastructure.Notifications;
using Showmatics.Blazor.Client.Infrastructure.Preferences;
using MediatR.Courier;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Showmatics.Blazor.Client.Components.Common;

public class FshTable<T> : MudTable<T>
{
    [Inject]
    private IClientPreferenceManager ClientPreferences { get; set; } = default!;
    [Inject]
    protected ICourier Courier { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (await ClientPreferences.GetPreference() is ClientPreference clientPreference)
        {
            SetTablePreference(clientPreference.TablePreference);
        }

        Courier.SubscribeWeak<NotificationWrapper<FshTablePreference>>(wrapper =>
        {
            SetTablePreference(wrapper.Notification);
            StateHasChanged();
        });

        await base.OnInitializedAsync();
    }

    private void SetTablePreference(FshTablePreference tablePreference)
    {
        Dense = true; // tablePreference.IsDense;
        Striped = true; // tablePreference.IsStriped;
        Bordered = tablePreference.HasBorder;
        Hover = tablePreference.IsHoverable;
    }
}
