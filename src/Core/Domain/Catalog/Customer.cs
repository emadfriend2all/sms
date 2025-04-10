using System;
using System.Collections.Generic;

namespace ShowMatic.Server.Domain.Catalog;

public partial class Customer : AuditableEntity<int>, IAggregateRoot
{
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? IdentityNumber { get; set; }
    public string? TaxNumber { get; set; }
    public string? RegistrationNo { get; set; }
    public bool? IsCompany { get; set; }
}
