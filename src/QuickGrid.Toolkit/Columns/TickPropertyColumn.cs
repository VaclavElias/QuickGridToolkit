namespace QuickGrid.Toolkit.Columns;

public class TickPropertyColumn<TGridItem> : DynamicColumn<TGridItem>
{
    public bool ShowOnlyTrue { get; set; }
}

// ToDo maybe an action should be part of DynamicColumn?
public class TogglePropertyColumn<TGridItem> : DynamicColumn<TGridItem>
{
    public bool ShowOnlyTrue { get; set; }
}