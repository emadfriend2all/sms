using ShowMatic.Server.Domain.Catalog;

namespace ShowMatic.Server.Application.Catalog.Customers.Specifications;

public class CustomerByPhoneNumberSpec : Specification<Customer>, ISingleResultSpecification
{
    public CustomerByPhoneNumberSpec(string phoneNumber) =>
        Query.Where(p => p.PhoneNumber == phoneNumber);
}