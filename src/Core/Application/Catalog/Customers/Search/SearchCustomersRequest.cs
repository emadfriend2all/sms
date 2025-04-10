using ShowMatic.Server.Application.Catalog.Customers.Specifications;

namespace ShowMatic.Server.Application.Catalog.Customers.Search;

public class SearchCustomersRequest : PaginationFilter, IRequest<PaginationResponse<GetCustomerResponse>>
{
    public string? IdentityNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public bool? IsCompany { get; set; }
}

public class SearchCustomersRequestHandler : IRequestHandler<SearchCustomersRequest, PaginationResponse<GetCustomerResponse>>
{
    private readonly IReadRepository<Customer> _repository;

    public SearchCustomersRequestHandler(IReadRepository<Customer> repository) => _repository = repository;

    public async Task<PaginationResponse<GetCustomerResponse>> Handle(SearchCustomersRequest request, CancellationToken cancellationToken)
    {
        var spec = new CustomersBySearchRequestWithBrandsSpec(request);
        var result = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
        return result;
    }
}