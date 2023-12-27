namespace QuickGridToolkit.Columns;

public class EmptyColumn<TGridItem> : ColumnBase<TGridItem>
{
    private readonly static RenderFragment<TGridItem> _emptyChildContent = _ => __ => { };

    public override GridSort<TGridItem>? SortBy { get; set; }

    [Parameter] public RenderFragment<TGridItem> ChildContent { get; set; } = _emptyChildContent;

    protected override void CellContent(RenderTreeBuilder builder, TGridItem item)
            => builder.AddContent(0, "");
}

//public class EmptyColumn<TGridItem> : ColumnBase<TGridItem>
//{
//    /// <summary>
//    /// Dummy Property otherwise it doesn't render
//    /// </summary>
//    [Parameter] public TGridItem? Property { get; set; }

//    protected override void CellContent(RenderTreeBuilder builder, TGridItem item) { }
//    //protected override void CellContent(RenderTreeBuilder builder, TGridItem item) => builder.AddContent(0, "Hi");
//}