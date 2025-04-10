namespace Showmatics.Blazor.Client.Models;

public partial class KanbanDataRecord
{
    public string Status { get; set; } = default!;
    public string Color { get; set; } = default!;
    public string Priority { get; set; } = default!;
    public string Text { get; set; } = default!;
    public string Tags { get; set; } = default!;
    public int Progress { get; set; }
    public int UserId { get; set; }
    public int Id { get; set; }
    public object[] CheckList { get; set; } = default!;
    public object[] Comments { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }

}
