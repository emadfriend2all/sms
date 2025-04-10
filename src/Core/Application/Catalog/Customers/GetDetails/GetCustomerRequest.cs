using ShowMatic.Server.Application.Catalog.Customers.Specifications;
using ShowMatic.Server.Domain.Catalog;

namespace ShowMatic.Server.Application.Catalog.Customers.GetDetails;

public class GetCustomerRequest : IRequest<GetCustomerDetailsResponse>
{
    public int Id { get; set; }
}

public class GetCustomerRequestHandler : IRequestHandler<GetCustomerRequest, GetCustomerDetailsResponse>
{
    private readonly IRepository<Customer> _repository;
    private readonly IStringLocalizer<GetCustomerRequestHandler> _localizer;

    public GetCustomerRequestHandler(IRepository<Customer> repository, IStringLocalizer<GetCustomerRequestHandler> localizer) =>
        (_repository, _localizer) = (repository, localizer);

    public async Task<GetCustomerDetailsResponse> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await _repository.FirstOrDefaultAsync(new CustomerByPhoneNumberWithContactsSpec(request), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["customer.notfound"], request.Id));
        return result;
    }
}