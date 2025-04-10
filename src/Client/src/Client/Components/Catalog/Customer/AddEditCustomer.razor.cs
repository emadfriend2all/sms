using Microsoft.AspNetCore.Components;
using Showmatics.Blazor.Client.Infrastructure.ApiClient;
using Mapster;
namespace Showmatics.Blazor.Client.Components.Catalog.Customer;

public partial class AddEditCustomer
{
    [Parameter]
    public UpdateCustomerRequest Request { get; set; } = default!;
    [Parameter]
    public EventCallback<UpdateCustomerRequest> RequestChanged { get; set; }
    protected CreateAddressRequest AddressRequest { get; set; } = default!;
}
