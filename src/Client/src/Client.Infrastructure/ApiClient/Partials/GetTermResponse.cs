namespace Showmatics.Blazor.Client.Infrastructure.ApiClient;

public partial class GetTermResponse
{
    public bool ReadOnly { get; set; }
    public int? AddedByFieldId { get; set; }
    public string OriginalTerm { get; set; } = default!;
}