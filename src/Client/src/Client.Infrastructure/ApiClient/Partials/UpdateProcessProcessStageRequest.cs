namespace Showmatics.Blazor.Client.Infrastructure.ApiClient;

public partial class UpdateProcessProcessStageRequest
{
    public bool IsOnEditMode { get; set; }
    public int CaseStatusIdValue
    {
        get => CaseStatusId ?? 0;
        set => CaseStatusId = value == default ? null : value;
    }

    public int ContractStatusIdValue
    {
        get => ContractStatusId ?? 0;
        set => ContractStatusId = value == default ? null : value;
    }

    public int OnSuccessStageIdValue
    {
        get => OnSuccessStageId ?? 0;
        set => OnSuccessStageId = value == default ? null : value;
    }

    public int OnFailureStageIdValue
    {
        get => OnFailureStageId ?? 0;
        set => OnFailureStageId = value == default ? null : value;
    }

    public int WorkGroupIdValue
    {
        get => OnFailureStageId ?? 0;
        set => OnFailureStageId = value == default ? null : value;
    }
}