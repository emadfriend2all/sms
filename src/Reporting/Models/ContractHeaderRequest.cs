namespace Reporting.Models;
public class ContractHeaderRequest
{
    public ContractHeaderRequest()
    {
    }

    public string? PrintDateTxt { get; set; }
    public string? DateInHigriTxt { get; set; }
    public string? DocumentNumberTxt { get; set; }
    public string? DocumentNameTxt { get; set; }
    public string? FirstPartyNameTxt { get; set; }
    public string? FirstPartyAddressTxt { get; set; }
    public string? FirstPartyRegistrationNoTxt { get; set; }
    public string? SecondPartyNameTxt { get; set; }
    public string? SecondPartyAddressTxt { get; set; }
    public string? SecondPartyIdTxt { get; set; }
    public string? SecondPartyPhoneTxt { get; set; }
    public string? Id { get; set; }
    public string? CustomerId { get; set; }
    public string? ReferenceSerialNumber { get; set; }
    public string? OwnerNameTxt { get; set; }
    public string? FirstPartyOwnerTitleTxt { get; set; }
    public string? SecondPartyIdLabelTxt { get; set; }
    public bool IncludeParties { get; set; }
}
