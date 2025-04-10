using ShowMatic.Server.Domain.Catalog;

namespace ShowMatic.Server.Application.Catalog.Customers.Specifications;

public class CustomerByIdWithTaxInfoSpec : Specification<Customer>, ISingleResultSpecification
{
    public CustomerByIdWithTaxInfoSpec(int id) =>
        Query
        .Where(p => p.Id == id);
}