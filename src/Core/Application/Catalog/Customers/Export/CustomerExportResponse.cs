namespace ShowMatic.Server.Application.Catalog.Customers.Export;

public class CustomerExportResponse : IResponseDto
{
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentityNumber { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNo { get; set; }
    public bool? IsCompany { get; set; }
    public string? City { get; set; }
    public string? FullAddress { get; set; }
}