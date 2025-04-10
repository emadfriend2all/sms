namespace Showmatics.Blazor.Client.Infrastructure.ApiClient;

public partial class AddEditOpenningBalanceLinesRequest
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string AccountClassName { get; set; } = default!;
    public decimal Balance { get; set; } = default!;
}
