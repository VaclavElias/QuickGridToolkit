namespace QuickGrid.Toolkit.Columns;

public class ToggleColumn<TGridItem> : ColumnBase<TGridItem>
{
    public override GridSort<TGridItem>? SortBy
    {
        get => _sortBuilder;
        set => throw new NotSupportedException($"PropertyColumn generates this member internally. For custom sorting rules, see '{typeof(TemplateColumn<TGridItem>)}'.");
    }

    [Parameter] public Expression<Func<TGridItem, object?>> Property { get; set; } = default!;
    [Parameter] public Func<TGridItem, Task>? OnChangeAsync { get; set; }

    private Expression<Func<TGridItem, object?>>? _lastAssignedProperty;
    private Func<TGridItem, object?>? _cellTextFunc;
    private GridSort<TGridItem>? _sortBuilder;

    //GridSort<TGridItem>? ISortBuilderColumn<TGridItem>.SortBuilder => _sortBuilder;

    protected override void CellContent(RenderTreeBuilder builder, TGridItem item)
    {
        var isTrue = _cellTextFunc!(item)?.ToString() == "True";
        var isNull = _cellTextFunc!(item) is null;

        isTrue = isTrue && !isNull;

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "form-switch d-inline-block");
        builder.OpenElement(2, "input");
        if (OnChangeAsync is not null)
        {
            builder.AddAttribute(3, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, () => OnChangeAsync.Invoke(item)));
        }
        builder.AddAttribute(4, "class", "form-check-input");
        builder.AddAttribute(5, "type", "checkbox");
        builder.AddAttribute(6, "role", "switch");
        if (isTrue)
        {
            builder.AddAttribute(7, "checked");
        }
        builder.CloseElement();
        builder.CloseElement();
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