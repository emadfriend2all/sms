using ShowMatic.Server.Domain.Catalog;

namespace ShowMatic.Server.Application.Catalog.Customers.GetDetails;

public class GetCustomerDetailsResponse : IResponseDto
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
    public ICollection<GetCustomerDetailsAddressResponse>? Addresses { get; set; }
    public ICollection<GetCustomerDetailsCaseResponse>? Cases { get; set; }
    public ICollection<GetCustomerDetailsContactResponse>? Contacts { get; set; }
}

public class GetCustomerDetailsCaseHistoryResponse
{
    public DateTimeOffset? Date { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
}

public class GetCustomerDetailsAddressResponse
{
    public int Id { get; set; }
    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? Suburb { get; set; }
    public string? StreetName { get; set; }
    public string? StreetNumber { get; set; }
    public string? LandMark { get; set; }
    public string? PostalCode { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? FullAddress { get; set; }
}

public class GetCustomerDetailsContactResponse
{
    public int Id { get; set; }
    public string? Position { get; set; }
    public string? Email { get; set; }
    public string PhoneNumber { get; set; } = null!;
}

public class GetCustomerDetailsCaseResponse
{
    public int Id { get; set; }
    public int? CategoryId { get; set; }
    public int? UnitId { get; set; }
    public int? UnitItemId { get; set; }
    public int? ProjectId { get; set; }
    public DateTimeOffset? Date { get; set; }
    public int? StatusId { get; set; }
    public string? Description { get; set; }
    public GetCustomerDetailsCaseCategoryResponse? Category { get; set; }
    public GetCustomerDetailsProjectStatusResponse? Project { get; set; }
    public GetCustomerDetailsCaseStatusResponse? Status { get; set; }
    public GetCustomerDetailsCaseCategoryUnitResponse? CategoryUnit { get; set; }
    public GetCustomerDetailsCaseCategoryUnitItemResponse? UnitItem { get; set; }
}
public class GetCustomerDetailsCaseCategoryResponse
{
    public string? Name { get; set; }
}

public class GetCustomerDetailsCaseCategoryUnitResponse
{
    public string? Name { get; set; }
}

public class GetCustomerDetailsCaseCategoryUnitItemResponse
{
    public string? Name { get; set; }
}

public class GetCustomerDetailsCaseStatusResponse
{
    public string? Name { get; set; }
    public string? Type { get; set; }
}

public class GetCustomerDetailsProjectStatusResponse
{
    public string? Name { get; set; }
}