namespace QuickGridToolkit;

public class ColumnManager<TGridItem>
{
    public readonly List<DynamicColumn<TGridItem>> Columns = new();

    public readonly QuickGridColumns QuickGridColumns = new();

    /// <summary>
    /// Returns visible columns
    /// </summary>
    /// <returns></returns>
    public IEnumerable<DynamicColumn<TGridItem>> Get() => Columns.Where(w => w.Visible);

    public void Add(DynamicColumn<TGridItem>? column = default)
    {
        if (column == null) return;

        if (string.IsNullOrWhiteSpace(column.Title))
            column.Title = GetPropertyName(column.Property) ?? "Title n/a";

        Columns.Add(column);

        column.Id = Columns.Count;
    }

    public void AddSimple(Expression<Func<TGridItem, object?>> expression)
    {
        Add(new() { Property = expression });
    }

    public void AddTickColumn(Expression<Func<TGridItem, object?>> expression, string? title = null, Align align = Align.Center)
    {
        Add(new() { Property = expression, ColumnType = typeof(TickColumn<TGridItem>), Title = title, Align = align });
    }

    public void AddImageColumn(Expression<Func<TGridItem, object?>> expression, string? title = null, Align align = Align.Center, string? @class = null)
    {
        Add(new() { Property = expression, ColumnType = typeof(ImageColumn<TGridItem>), Title = title, Align = align, Class = @class });
    }

    public void AddTemplateColumn(RenderFragment<TGridItem> childContent, string? title = null, Align align = Align.Center, GridSort<TGridItem>? sortBy = null)
    {
        Add(new() { ChildContent = childContent, ColumnType = typeof(TemplateColumn<TGridItem>), Title = title, Align = align, SortBy = sortBy });
    }

    public void AddTemplateColumn2(Expression<Func<TGridItem, object?>> expression, string? title = null, Align align = Align.Center, GridSort<TGridItem>? sortBy = null)
    {
        Add(new() { ChildContent = QuickGridColumns.GetActionColumn(expression), ColumnType = typeof(TemplateColumn<TGridItem>), Title = title, Align = align, SortBy = sortBy });
    }

    public void AddIndexColumn(string title = "#", Align align = Align.Center)
        => Add(new() { ColumnType = typeof(EmptyColumn<TGridItem>), Title = title, Align = align });

    private static string? GetPropertyName(Expression<Func<TGridItem, object?>>? expression)
    {
        if (expression is null) return null;

        MemberExpression? memberExpression;

        if (expression.Body is UnaryExpression unaryExpression)
            memberExpression = unaryExpression.Operand as MemberExpression;
        else
            memberExpression = expression.Body as MemberExpression;

        if (memberExpression == null)
            throw new ArgumentException($"Expression '{expression}' refers to a method, not a property.");

        if (!(memberExpression.Member is PropertyInfo propertyInfo))
            throw new ArgumentException($"Expression '{expression}' refers to a field, not a property.");

        return propertyInfo.Name;
    }
}