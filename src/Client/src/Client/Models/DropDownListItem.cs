namespace Showmatics.Blazor.Client.Models;

public class DropDownListItem<TId>
{
    public string Text { get; set; } = default!;
    public TId Value { get; set; } = default!;
}
