using Microsoft.AspNetCore.Components;
using Showmatics.Blazor.Client.Infrastructure.ApiClient;

namespace Showmatics.Blazor.Client.Pages.Multitenancy;

public partial class AddEditTenant
{
    [Parameter]
    public CreateTenantRequest Request { get; set; } = new();
    [Parameter]
    public EventCallback<CreateTenantRequest> RequestChanged { get; set; }
    private string SelectedModules { get; set; } = string.Empty;

    private IEnumerable<string> Modules
    {
        get
        {
            return Request.TenantModules.AsEnumerable();
        }
        set
        {
            Request.TenantModules = value.ToList();
        }
    }
}
