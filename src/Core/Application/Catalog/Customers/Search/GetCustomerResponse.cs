namespace ShowMatic.Server.Application.Catalog.Customers.Search;

public class GetCustomerResponse : IResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentityNumber { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNo { get; set; }
    public bool IsCompany { get; set; }
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
    public ICollection<GetCustomerAddressResponse>? Addresses { get; set; }
    public ICollection<GetCustomerCaseResponse>? Cases { get; set; }
    public ICollection<GetCustomerContactResponse>? Contacts { get; set; }
}

public class GetCustomerCaseHistoryResponse
{
    public DateTimeOffset? Date { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
}

public class GetCustomerAddressResponse
{
    public int Id { get; set; }
    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? Suburb { get; set; }
    public string? StreetName { get; set; }
    public string? StreetNumber { get; set; }
    public string? LandMark { get; set; }
    public string? FullAddress { get; set; }
    public string? ShortAddress { get; set; }
    public string? PostalCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class GetCustomerContactResponse
{
    public int Id { get; set; }
    public string? Position { get; set; }
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = null!;
}

public class GetCustomerCaseResponse
{
    public int Id { get; set; }
    public DateTimeOffset? Date { get; set; }
    public int? CategoryId { get; set; }
    public int? CategoryUnitId { get; set; }
    public int? UnitItemId { get; set; }
    public int? ProjectId { get; set; }
    public int? CustomerId { get; set; }
    public int? StatusId { get; set; }
    public string? Description { get; set; }
    public GetCustomerCaseCategoryResponse? Category { get; set; }
    public GetCustomerCaseStatusResponse? Status { get; set; }
    public GetCustomerCategoryUnitResponse? CategoryUnit { get; set; }
    public GetCustomerUnitItemResponse? UnitItem { get; set; }
}

public class GetCustomerCaseCategoryResponse
{
    public string? Name { get; set; }
}

public class GetCustomerCategoryUnitResponse
{
    public string? Name { get; set; }
}

public class GetCustomerUnitItemResponse
{
    public string? Name { get; set; }
}

public class GetCustomerCaseStatusResponse
{
    public string? Name { get; set; }
}