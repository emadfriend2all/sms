using ShowMatic.Server.Application.Catalog.Customers.GetDetails;
using ShowMatic.Server.Domain.Catalog;

namespace ShowMatic.Server.Application.Catalog.Customers.Specifications;

public class CustomerByPhoneNumberWithContactsSpec : Specification<Customer, GetCustomerDetailsResponse>, ISingleResultSpecification
{
    public CustomerByPhoneNumberWithContactsSpec(GetCustomerRequest request) =>
        Query
            .Where(c => c.Id == request.Id);
}

public class CustomerFinancialInfoSpec : Specification<Customer>, ISingleResultSpecification
{
    public CustomerFinancialInfoSpec(int id) =>
        Query
            .Where(c => c.Id == id);
}