using MudBlazor;

namespace Showmatics.Blazor.Client.Components.Common;

public class FSHTextField<T> : MudTextField<T>
{
    protected override void OnInitialized()
    {
        Margin = Margin.Dense;
        Variant = Variant.Outlined;
    }
}