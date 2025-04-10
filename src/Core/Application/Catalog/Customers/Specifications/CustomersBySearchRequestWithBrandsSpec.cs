using Ardalis.Specification;
using ShowMatic.Server.Application.Catalog.Customers.Search;
using ShowMatic.Server.Domain.Catalog;

namespace ShowMatic.Server.Application.Catalog.Customers.Specifications;

public class CustomersBySearchRequestWithBrandsSpec : EntitiesByPaginationFilterSpec<Customer, GetCustomerResponse>
{
    public CustomersBySearchRequestWithBrandsSpec(SearchCustomersRequest request)
        : base(request) =>
        Query
            .OrderByDescending(c => c.Id, !request.HasOrderBy())
            .Where(p => p.IdentityNumber != null && p.IdentityNumber.Contains(request.IdentityNumber!), !string.IsNullOrEmpty(request.IdentityNumber));
}