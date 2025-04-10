using ShowMatic.Server.Application.Catalog.Customers.Export;
using ShowMatic.Server.Domain.Catalog;

namespace ShowMatic.Server.Application.Catalog.Customers.Specifications;

public class ExportCustomersWithContactsSpecification : EntitiesByBaseFilterSpec<Customer>
{
    public ExportCustomersWithContactsSpecification(ExportCustomersRequest request)
        : base(request)
    {
        Query
            .Where(p => p.IsCompany.Equals(request.IsCompany!.Value), request.IsCompany.HasValue);
    }
}