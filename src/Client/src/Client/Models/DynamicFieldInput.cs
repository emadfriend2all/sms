namespace Showmatics.Blazor.Client.Models;

public class DynamicFieldInput
{
    public int Id { get; set; }
    public string Label { get; set; }
    public string? Type { get; set; }
    public bool IsRequired { get; set; }
    public bool Disabled { get; set; }
    public int? Width { get; set; }
    public string? Regex { get; set; }
    public string? ContentType { get; set; }
    public string? DefaultValue { get; set; }
    public int? FormId { get; set; }
    public ICollection<DynamicFieldInputOption> FieldOptions { get; set; } = new List<DynamicFieldInputOption>();
}

public class DynamicFieldInputOption
{
    public string? Name { get; set; }
    public string? Value { get; set; }
}