namespace QuickGrid.Toolkit.Columns;

public class ToggleColumn<TGridItem> : ColumnBase<TGridItem>
{
    public override GridSort<TGridItem>? SortBy
    {
        get => _sortBuilder;
        set => throw new NotSupportedException($"PropertyColumn generates this member internally. For custom sorting rules, see '{typeof(TemplateColumn<TGridItem>)}'.");
    }

    [Parameter] public Expression<Func<TGridItem, object>> Property { get; set; } = default!;
    [Parameter] public EventCallback<TGridItem?>? OnChange { get; set; }

    private Expression<Func<TGridItem, object>>? _lastAssignedProperty;
    private Func<TGridItem, object?>? _cellTextFunc;
    private GridSort<TGridItem>? _sortBuilder;

    //GridSort<TGridItem>? ISortBuilderColumn<TGridItem>.SortBuilder => _sortBuilder;

    protected override void CellContent(RenderTreeBuilder builder, TGridItem item)
    {
        var isTrue = _cellTextFunc!(item)?.ToString() == "True";
        var isNull = _cellTextFunc!(item) is null;

        //if (isNull)
        //{
        //    builder.AddContent(0, string.Empty);
        //    return;
        //}

        //if (ShowOnlyTrue && !isTrue)
        //{
        //    builder.AddContent(0, string.Empty);
        //    return;
        //}

        isTrue = isTrue && !isNull;

        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "form-switch d-inline-block");
        if (OnChange is not null)
        {
            builder.AddAttribute(2, "onchange", EventCallback.Factory.Create(this, () =>
            {
                Console.WriteLine($"ToggleColumn clicked for item: {item}");
                OnChange.Value.InvokeAsync(item);
            }));
        }
        builder.OpenElement(3, "input");
        builder.AddAttribute(4, "class", "form-check-input");
        builder.AddAttribute(5, "type", "checkbox");
        builder.AddAttribute(6, "role", "switch");
        if (isTrue)
        {
            builder.AddAttribute(7, "checked");
        }
        builder.CloseElement();
        builder.CloseElement();

        // form-ckeck class is removed because it adds extra padding
        //builder.AddMarkupContent(2, $"<div class=\"form-switch d-inline-block\"><input {(isTrue ? "checked" : "")} class=\"form-check-input\" type=\"checkbox\" role=\"switch\"></div>");
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