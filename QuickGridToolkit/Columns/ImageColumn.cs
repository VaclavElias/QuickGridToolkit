namespace QuickGridToolkit.Columns;

public class ImageColumn<TGridItem> : ColumnBase<TGridItem>
{
    public override GridSort<TGridItem>? SortBy { get; set; }

    [Parameter] public Expression<Func<TGridItem, object>> Property { get; set; } = default!;

    private Expression<Func<TGridItem, object>>? _lastAssignedProperty;
    private Func<TGridItem, object?>? _cellTextFunc;

    protected override void CellContent(RenderTreeBuilder builder, TGridItem item)
    {
        var imagePath = _cellTextFunc!(item)?.ToString();

        builder.AddMarkupContent(2, $"<img alt=\"\" src=\"{imagePath}\">");
    }

    //ToDo - This is a copy of the code in TickColumn.cs, we should refactor/merge this?
    protected override void OnParametersSet()
    {
        if (_lastAssignedProperty != Property)
        {
            _lastAssignedProperty = Property;

            var compiledPropertyExpression = Property.Compile();

            _cellTextFunc = item => compiledPropertyExpression!(item)?.ToString();
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
    }
}
