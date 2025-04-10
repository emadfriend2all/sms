using Mapster;
using Microsoft.AspNetCore.Components;
using Showmatics.Blazor.Client.Components.EntityTable;
using Showmatics.Blazor.Client.Infrastructure.ApiClient;
using Showmatics.Shared.Authorization;

namespace Showmatics.Blazor.Client.Pages.Catalog;

public partial class Customers
{
    [Inject]
    private ICustomersClient CustomersClient { get; set; } = default!;


    protected EntityServerTableContext<GetCustomerResponse, int, UpdateCustomerRequest> Context { get; set; } = default!;
    protected EntityTable<GetCustomerResponse, int, UpdateCustomerRequest> _table = default!;
    public ICollection<CreateAddressRequest> Addresses { get; set; } = default!;
    protected string AddressSearchString { get; set; } = default!;
    protected bool showDetails = true;
    protected override void OnInitialized()
    {
        Context = new(
            entityName: L["Customer"],
            entityNamePlural: L["Customers"],
            enableAdvancedSearch: true,
            entityResource: FSHResource.Customers,
            fields: new()
                                {
                new(customer => customer.Id, L["Id"], "Id"),
                new(customer => customer.Name, L["Name"], "Name"),
                new(customer => customer.IdentityNumber, L["IdentityNumber"], "IdentityNumber"),
                new(customer => customer.PhoneNumber, L["PhoneNumber"], "PhoneNumber"),
                new(customer => customer.TaxNumber, L["TaxNumber"], "TaxNumber"),
                new(customer => customer.RegistrationNo, L["RegistrationNo"], "RegistrationNo"),
                new(customer => customer.IdentityNumber, L["IdentityNumber"], "IdentityNumber"),
                new(customer => customer.Address, L["Address"], "Address"),

                                },
            idFunc: customer => customer.Id,
            searchFunc: async filter =>
            {
                var customerFilter = filter.Adapt<SearchCustomersRequest>();

                customerFilter.PhoneNumber = SearchPhoneNumber;
                customerFilter.IdentityNumber = SearchIdentity;
                customerFilter.Address = SearchAddress;

                var result = await CustomersClient.SearchAsync(customerFilter);
                return result.Adapt<PaginationResponse<GetCustomerResponse>>();
            },
            createFunc: async customer => await CreateCustomerAsync(customer),
            updateFunc: async (id, customer) => await UpdateCustomerAsync(id, customer),
            deleteFunc: async id => await CustomersClient.DeleteAsync(id),

            exportAction: string.Empty);
    }

    private async Task CreateCustomerAsync(UpdateCustomerRequest customer)
    {

        await CustomersClient.CreateAsync(customer.Adapt<CreateCustomerRequest>());
    }

    private async Task UpdateCustomerAsync(int id, UpdateCustomerRequest customer)
    {
        await CustomersClient.UpdateAsync(id, customer);
    }

    protected void ToggleShowDetails()
    {
        showDetails = !showDetails;
    }

    protected string? _searchIdentity;
    protected string? SearchIdentity
    {
        get => _searchIdentity;
        set
        {
            _searchIdentity = value;
            _ = _table.ReloadDataAsync();
        }
    }

    protected string? _searchPhoneNumber;
    protected string? SearchPhoneNumber
    {
        get => _searchPhoneNumber;
        set
        {
            _searchPhoneNumber = value;
            _ = _table.ReloadDataAsync();
        }
    }

    protected string? _searchAddress;
    protected string? SearchAddress
    {
        get => _searchAddress;
        set
        {
            _searchAddress = value;
            _ = _table.ReloadDataAsync();
        }
    }

}
