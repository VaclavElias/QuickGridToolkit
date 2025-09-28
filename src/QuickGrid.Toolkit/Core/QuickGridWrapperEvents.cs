namespace QuickGrid.Toolkit.Core;

/// <summary>
/// Provides event callbacks for user interactions and actions within a QuickGrid wrapper component, enabling consumers
/// to handle warnings, view selection, column management, view resets, and data export operations.
/// </summary>
/// <remarks>Use these events to respond to grid-related actions such as exporting data, managing columns, or
/// handling user warnings. All events are intended to be initialized when configuring the grid wrapper and are
/// typically invoked by the component in response to user actions.</remarks>
/// <typeparam name="TGridItem">The type of items displayed in the grid. Specifies the data model for each row in the grid.</typeparam>
public class QuickGridWrapperEvents<TGridItem>
{
    public EventCallback<string> WarningRequested { get; set; }
    public EventCallback<ColumnConfig> OnSelectView { get; set; }
    public EventCallback OnManageColumns { get; set; }
    public EventCallback OnResetViewToDefault { get; set; }
    public EventCallback<IQueryable<TGridItem>> OnExport { get; set; }
    public EventCallback<IEnumerable<IDictionary<string, object?>?>?> OnSelectedColumnsExport { get; set; }
}