namespace QuickGridToolkit.Columns;

// source https://github.com/aspnet/AspLabs/blob/main/src/QuickGrid/src/Microsoft.AspNetCore.Components.QuickGrid/Columns/PropertyColumn.cs
public class TickColumn<TGridItem> : ColumnBase<TGridItem>
{
    public override GridSort<TGridItem>? SortBy { get; set; }

    [Parameter] public Expression<Func<TGridItem, object>> Property { get; set; } = default!;

    private Expression<Func<TGridItem, object>>? _lastAssignedProperty;
    private Func<TGridItem, object?>? _cellTextFunc;
    private GridSort<TGridItem>? _sortBuilder;

    //GridSort<TGridItem>? ISortBuilderColumn<TGridItem>.SortBuilder => _sortBuilder;

    protected override void CellContent(RenderTreeBuilder builder, TGridItem item)
    {
        var isTrue = _cellTextFunc!(item) == "True";

        builder.AddMarkupContent(2, $"<i class=\"far fa-{(isTrue ? "check" : "times")} text-{(isTrue ? "success" : "danger")}\"></i>");
        //builder.OpenElement(0, "div");
        //builder.AddAttribute(1, "class", "form-check");
        //builder.AddMarkupContent(2, $"<label class=\"form-check-label\">{_cellTextFunc!(item)}</label>");
        //builder.AddContent(3, " Hello");
        //builder.CloseElement();

        //builder.OpenComponent<Checkbox>(0);
        //builder.AddAttribute(1, "Value", item);
        //builder.CloseComponent();
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
        if (Title is not null) return;

        MemberExpression? memberExpression;

        if (Property.Body is UnaryExpression unaryExpression)
            memberExpression = unaryExpression.Operand as MemberExpression;
        else
            memberExpression = Property.Body as MemberExpression;

        if (memberExpression == null)
            throw new ArgumentException($"Expression '{Property}' refers to a method, not a property.");

        if (memberExpression.Member is not PropertyInfo propertyInfo)
            throw new ArgumentException($"Expression '{Property}' refers to a field, not a property.");

        Title = propertyInfo.Name;

        //if (Title is null && Property.Body is MemberExpression memberExpression)
        //{
        //    Title = memberExpression.Member.Name;
        //}
    }
}
