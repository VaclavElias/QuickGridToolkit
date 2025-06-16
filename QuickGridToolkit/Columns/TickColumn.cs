namespace QuickGridToolkit.Columns;

// source https://github.com/aspnet/AspLabs/blob/main/src/QuickGrid/src/Microsoft.AspNetCore.Components.QuickGrid/Columns/PropertyColumn.cs
public class TickColumn<TGridItem> : ColumnBase<TGridItem>
{
    public override GridSort<TGridItem>? SortBy
    {
        get => _sortBuilder;
        set => throw new NotSupportedException($"PropertyColumn generates this member internally. For custom sorting rules, see '{typeof(TemplateColumn<TGridItem>)}'.");
    }

    [Parameter] public Expression<Func<TGridItem, object>> Property { get; set; } = default!;
    [Parameter] public bool ShowOnlyTrue { get; set; }

    private Expression<Func<TGridItem, object>>? _lastAssignedProperty;
    private Func<TGridItem, object?>? _cellTextFunc;
    private GridSort<TGridItem>? _sortBuilder;

    //GridSort<TGridItem>? ISortBuilderColumn<TGridItem>.SortBuilder => _sortBuilder;

    protected override void CellContent(RenderTreeBuilder builder, TGridItem item)
    {
        var isTrue = _cellTextFunc!(item)?.ToString() == "True";
        var isNull = _cellTextFunc!(item) is null;

        if (isNull)
        {
            builder.AddContent(0, string.Empty);
            return;
        }

        if (ShowOnlyTrue && !isTrue)
        {
            builder.AddContent(0, string.Empty);

            return;
        }

        builder.AddMarkupContent(2, $"<i class=\"far fa-{(isTrue ? "check" : "times")} text-{(isTrue ? "success" : "danger")}\"></i>");
    }

    protected override void OnParametersSet()
    {
        if (_lastAssignedProperty != Property)
        {
            _lastAssignedProperty = Property;

            var compiledPropertyExpression = Property.Compile();

            _cellTextFunc = item => compiledPropertyExpression!(item)?.ToString();

            _sortBuilder = GridSort<TGridItem>.ByAscending(Property);
        }

        if (Title is null && Property.Body is MemberExpression memberExpression)
        {
            Title = memberExpression.Member.Name;
        }
    }
}