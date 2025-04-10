using Mapster;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Showmatics.Blazor.Client.Components.EntityTable;

public partial class EditableEntityTable<TEntity>
    where TEntity : class, new()
{
    [Parameter]
    [EditorRequired]
    public EditableEntityTableContext<TEntity> Context { get; set; } = default!;
    [Parameter]
    [EditorRequired]
    public EditableEntityTableContext<TEntity> ChildContext { get; set; } = default!;
    [Parameter]
    public TEntity ViewContext { get; set; } = default!;

    [Parameter]
    public TEntity EditContext { get; set; } = default!;

    [Parameter]
    public RenderFragment<TEntity> EditRowTemplate { get; set; } = default!;
    [Parameter]
    public RenderFragment<TEntity> ViewRowTemplate { get; set; } = default!;
    [Parameter]
    public ICollection<TEntity>? Data { get; set; }
    [Parameter]
    public string? Title { get; set; }
    [Parameter]
    public string? Icon { get; set; }
    [Parameter]
    public EventCallback<MouseEventArgs> AddNewFunc { get; set; }
    private TEntity? _selectedItem = null;
    private TEntity? _elementBeforeEdit = null;
    protected override void OnInitialized()
    {
        if (Data is null)
        {
            Data = new List<TEntity>();
        }

        StateHasChanged();
    }

    private void Remove(TEntity entity)
    {
        Data?.Remove(entity);
        StateHasChanged();
    }

    private void Edit(object entity)
    {
        var modifiedEntity = (TEntity)entity;
        var current = Data?.FirstOrDefault(x => x == modifiedEntity);
        modifiedEntity.Adapt(current);
        Snackbar.Add($"Edited Successfuly {current}");
        StateHasChanged();
    }

    

    private void Reload(object entity)
    {
        StateHasChanged();
    }

    private void BackupItem(object entity)
    {
        _selectedItem = (TEntity)entity;
    }

    private void AddNew()
    {
        Data?.Add(new());
    }

    private void ResetItemToOriginalValues(object element)
    {
        element = _elementBeforeEdit!;
    }
}