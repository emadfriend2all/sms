using ShowMatic.Server.Domain.Catalog;

namespace ShowMatic.Server.Application.Catalog.Customers.Create;

public class CreateCustomerRequestValidator : CustomValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator(IReadRepository<Customer> customerRepo, IStringLocalizer<CreateCustomerRequestValidator> localizer)
    {
        RuleFor(p => p.Name)
            .NotEmpty();

        RuleFor(p => p.IsCompany)
            .NotEmpty();

        RuleFor(p => p.Addresses)
            .NotEmpty();

        RuleFor(p => p.Contacts)
            .NotEmpty();
    }
}