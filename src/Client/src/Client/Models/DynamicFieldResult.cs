using Microsoft.AspNetCore.Components.Forms;

namespace Showmatics.Blazor.Client.Models;

public class DynamicFieldResult
{
    public int FieldId { get; set; }
    public string? FieldText { get; set; }
    public string? FieldValue { get; set; } = string.Empty;
    public int? FormId { get; set; }
    public ICollection<IBrowserFile>? Files { get; set; } = new List<IBrowserFile>();
}
