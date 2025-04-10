using Showmatics.Shared.Authorization;
using MudBlazor;
using Mapster;

namespace Showmatics.Blazor.Client.Components.EntityTable;

public class EditableEntityTableContext<TEntity>
{
    public List<TEntity> Items { get; }
    public TEntity Model { get; }
    public List<TEntity> ItemsInEdit { get; }
    public List<EntityField<TEntity>> Fields { get; }
    public EditableEntityTableContext(List<TEntity> items, List<EntityField<TEntity>> fields, TEntity model)
    {
        Items = items;
        ItemsInEdit = new List<TEntity>();
        Fields = fields;
        Model = model;
    }

    public void AddItem(TEntity entity)
    {
        TEntity newItem = Activator.CreateInstance<TEntity>();
        ItemsInEdit.Add(newItem);
    }

    public void RemoveItem()
    {
        TEntity newItem = Activator.CreateInstance<TEntity>();
        ItemsInEdit.Add(newItem);
    }
}