namespace ShowMatic.Server.Application.Catalog.Customers.Update;

public class UpdateCustomerRequestValidator : CustomValidator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator(IReadRepository<Customer> custpmerRepo, IStringLocalizer<UpdateCustomerRequestValidator> localizer)
    {
        RuleFor(p => p.Name).NotEmpty();
    }
}