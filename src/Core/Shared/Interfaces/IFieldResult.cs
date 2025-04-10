namespace Showmatics.Shared.Interfaces;
public interface IFieldResult
{
    int FieldId { get; set; }
    string FieldText { get; set; }
    string FieldValue { get; set; }
    string FormName { get; set; }

}
