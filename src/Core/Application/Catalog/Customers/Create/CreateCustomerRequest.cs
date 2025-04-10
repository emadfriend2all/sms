using Mapster;
using Showmatics.Domain.Common.Events;

namespace ShowMatic.Server.Application.Catalog.Customers.Create;

public class CreateCustomerRequest : IRequest<int>
{
    public string Name { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentityNumber { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNo { get; set; }
    public bool IsCompany { get; set; }
    public ICollection<CreateCustomerAddressRequest> Addresses { get; set; } = default!;
    public ICollection<CreateCustomerContactRequest> Contacts { get; set; } = default!;

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

public class CreateCustomerAddressRequest
{
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

public class CreateCustomerContactRequest
{
    public string? Position { get; set; }
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = null!;
}

public class CreateCustomerRequestHandler : IRequestHandler<CreateCustomerRequest, int>
{
    private readonly IRepository<Customer> _repository;

    public CreateCustomerRequestHandler(IRepository<Customer> repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var customer = request.Adapt<Customer>();

        var address = request.Addresses.LastOrDefault();
        customer.Address = $"{address.Province}-{address.City}-{address.Suburb}";
        customer.PhoneNumber = request.Contacts.LastOrDefault().PhoneNumber;
        customer.DomainEvents.Add(EntityCreatedEvent.WithEntity(customer));

        await _repository.AddAsync(customer, cancellationToken);
        return customer.Id;
    }
}