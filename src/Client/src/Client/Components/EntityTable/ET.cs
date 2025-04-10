namespace Showmatics.Blazor.Client.Components.EntityTable;

using MudBlazor;
using Showmatics.Blazor.Client.Infrastructure.ApiClient;

//public class CustomMudTable<TEntity> : MudTable<TEntity>
//{
//    public CustomMudTable()
//    {
//        Elevation = 0;
//        Dense = true;
//        Striped = true;
//        ReadOnly = false;
//        CanCancelEdit = true;
//        Loading = false;
//        SortLabel = "Sort By";
//        CommitEditTooltip = "Commit Edit";
//        RowEditPreview = BackupItem;
//        RowEditCancel = ResetItemToOriginalValues;
//        RowEditCommit = Edit;
//        IsEditRowSwitchingBlocked = true;
//        ApplyButtonPosition = TableApplyButtonPosition.Start;
//        EditButtonPosition = TableEditButtonPosition.Start;
//        EditTrigger = TableEditTrigger.RowClick;
//    }

//    private void Edit(object obj)
//    {
//        throw new NotImplementedException();
//    }

//    private void ResetItemToOriginalValues(object obj)
//    {
//        throw new NotImplementedException();
//    }

//    private void BackupItem(object obj)
//    {
//        throw new NotImplementedException();
//    }

//    public void AddToolBarContent(string title, string icon)
//    {
//        ToolBarContent = builder =>
//        {
//            builder.OpenComponent<MudText>(0);
//            builder.AddAttribute(1, "Typo", "Typo.h6");
//            builder.OpenComponent<MudIcon>(2);
//            builder.AddAttribute(3, "Icon", icon);
//            builder.AddAttribute(4, "Class", "me-3 mb-n1");
//            builder.CloseComponent();
//            builder.AddAttribute(5, "ChildContent", title);
//            builder.CloseComponent();
//            builder.OpenComponent<MudSpacer>(6);
//            builder.CloseComponent();
//            builder.OpenComponent<MudIconButton>(7);
//            builder.AddAttribute(8, "Size", Size.Small);
//            builder.AddAttribute(9, "Icon", Icons.Material.Outlined.Add);
//            builder.AddAttribute(10, "Class", "pa-0");
//            //builder.AddAttribute(11, "OnClick", AddNew);
//            builder.CloseComponent();
//        };
//    }

//    public void AddHeaderContent(IEnumerable<EntityField<TEntity>> fields)
//    {
//        HeaderContent = builder =>
//        {
//            if (fields != null)
//            {
//                foreach (var field in fields)
//                {
//                    builder.OpenComponent<MudTh>(0);
//                    builder.OpenComponent<MudTableSortLabel<TEntity>(1);
//                    builder.AddAttribute(2, "SortBy", field.ValueFunc);
//                    builder.AddAttribute(3, "ChildContent", field.DisplayName);
//                    builder.CloseComponent();
//                    builder.CloseComponent();
//                }
//            }
//            builder.OpenComponent<MudTh>(4);
//            builder.AddAttribute(5, "Style", "text-align:right");
//            builder.CloseComponent();
//        };
//    }

//    public void AddRowTemplate(IEnumerable<EntityField<TEntity>> fields)
//    {
//        RowTemplate = builder =>
//        {
//            foreach (var field in fields)
//            {
//                builder.OpenComponent<MudTd>(0);
//                builder.AddAttribute(1, "DataLabel", field.DisplayName);
//                if (field.Type == typeof(bool))
//                {
//                    builder.OpenComponent<MudCheckBox>(2);
//                    builder.AddAttribute(3, "DataLabel", field.SortLabel);
//                    builder.AddAttribute(4, "Checked", field.ValueFunc(context));
//                    builder.AddAttribute(5, "ReadOnly", true);
//                    builder.AddAttribute(6, "Color", Color.Secondary);
//                    builder.CloseComponent();
//                }
//                else
//                {
//                    builder.OpenComponent<MudText>(7);
//                    builder.AddAttribute(8, "T", field.Type);
//                    builder.AddAttribute(9, "ChildContent", field.ValueFunc(context)?.ToString());
//                    builder.CloseComponent();
//                }
//                builder.CloseComponent();
//            }

//            builder.OpenComponent<MudTd>(10);
//            builder.OpenComponent<MudIconButton>(11);
//            builder.AddAttribute(12, "Size", Size.Small);
//            builder.AddAttribute(13, "Color", Color.Error);
//            builder.AddAttribute(14, "Icon", Icons.Material.Outlined.Delete);
//            builder.AddAttribute(15, "Class", "pa-0");
//            builder.AddAttribute(16, "OnClick", () => Remove(context));
//            builder.CloseComponent();
//            builder.CloseComponent();

//            builder.OpenComponent<MudTd>(17);
//            builder.AddAttribute(18, "DataLabel", "Actions");
//            builder.AddAttribute(19, "Style", "text-align: right");
//            builder.CloseComponent();
//            builder.OpenComponent<MudIconButton>(20);
//            builder.AddAttribute(21, "Size", Size.Small);
//            builder.AddAttribute(22, "Icon", Icons.Material.Outlined.Add);
//            builder.AddAttribute(23, "Class", "pa-0");
//            builder.AddAttribute(24, "OnClick", AddNew);
//            builder.CloseComponent();
//            builder.CloseComponent();
//        };
//    }

//    public void AddRowEditingTemplate(IEnumerable<EntityField<TEntity>> fields)
//    {
//        RowEditingTemplate = builder =>
//        {
//            foreach (var field in fields)
//            {
//                builder.OpenComponent<MudTd>(0);
//                builder.AddAttribute(1, "DataLabel", field.DisplayName);

//                if (field.Template != null)
//                {
//                    builder.AddContent(2, field.Template(context));
//                }
//                else if (field.Type == typeof(bool))
//                {
//                    builder.OpenComponent<MudCheckBox>(3);
//                    builder.AddAttribute(4, "DataLabel", field.SortLabel);
//                    builder.AddAttribute(5, "For", () => field.ValueFunc(context));
//                    builder.AddAttribute(6, "ReadOnly", true);
//                    builder.AddAttribute(7, "Color", Color.Secondary);
//                    builder.CloseComponent();
//                }
//                else if (field.Type == typeof(DateTime))
//                {
//                    builder.OpenComponent<MudDatePicker>(8);
//                    builder.AddAttribute(9, "DataLabel", field.SortLabel);
//                    builder.AddAttribute(10, "For", () => date);
//                    builder.AddAttribute(11, "Variant", Variant.Outlined);
//                    builder.AddAttribute(12, "PickerVariant", PickerVariant.Dialog);
//                    builder.AddAttribute(13, "DateFormat", "dd-MM-yyyy");
//                    builder.AddAttribute(14, "Date", date);
//                    builder.CloseComponent();
//                }
//                else
//                {
//                    var BoundValue = field.ValueFunc(context).ToString();
//                    builder.OpenComponent<MudTextField>(15);
//                    builder.AddAttribute(16, "DataLabel", field.SortLabel);
//                    builder.AddAttribute(17, "Value", BoundValue);
//                    builder.AddAttribute(18, "Variant", Variant.Outlined);
//                    builder.AddAttribute(19, "Label", field.DisplayName);
//                    builder.AddAttribute(20, "Margin", Margin.Dense);
//                    builder.CloseComponent();
//                }

//                builder.CloseComponent();
//            }
//        };
//    }

//    // Add any other methods or properties needed for your custom MudTable.
//}

