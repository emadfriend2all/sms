using Mapster;
using Showmatics.Domain.Common.Events;

namespace ShowMatic.Server.Application.Catalog.Customers.Update;

public class UpdateCustomerRequest : IRequest<int>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentityNumber { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNo { get; set; }
    public bool IsCompany { get; set; }
    public ICollection<UpdateCustomerAddressRequest> Addresses { get; set; } = default!;
    public ICollection<UpdateCustomerContactRequest> Contacts { get; set; } = default!;

    public string? No { get; set; }
    public int? AccountsReceivableAccountId { get; set; }
    public int? SalesAccountId { get; set; }
    public int? SalesDiscountAccountId { get; set; }
    public int? PromptPaymentDiscountAccountId { get; set; }
    public int? CustomerAdvancesAccountId { get; set; }
    public int? TaxGroupId { get; set; }
    public int? PaymentTermId { get; set; }
    public decimal Balance { get; set; }
    public string? TaxGroup { get; set; }
}

public class UpdateCustomerAddressRequest
{
    public int Id { get; set; }
    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? Suburb { get; set; }
    public string? StreetName { get; set; }
    public string? StreetNumber { get; set; }
    public string? LandMark { get; set; }
    public string FullAddress { get; set; } = default!;
    public string ShortAddress { get; set; } = default!;
    public string? PostalCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class UpdateCustomerContactRequest
{
    public int Id { get; set; }
    public string? Position { get; set; }
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = null!;
}

public class UpdateCustomerRequestHandler : IRequestHandler<UpdateCustomerRequest, int>
{
    private readonly IRepository<Customer> _repository;
    private readonly IStringLocalizer<UpdateCustomerRequestHandler> _localizer;

    public UpdateCustomerRequestHandler(IRepository<Customer> repository, IStringLocalizer<UpdateCustomerRequestHandler> localizer) =>
        (_repository, _localizer) = (repository, localizer);

    public async Task<int> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = customer ?? throw new NotFoundException(string.Format(_localizer["customer.notfound"], request.Id));
        request.Adapt(customer);

        // Add Domain Events to be raised after the commit
        customer.DomainEvents.Add(EntityUpdatedEvent.WithEntity(customer));

        await _repository.UpdateAsync(customer, cancellationToken);

        return request.Id;
    }
}