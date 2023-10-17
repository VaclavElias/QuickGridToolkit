namespace QuickGridToolkit;

public class ColumnManager<TGridItem>
{
    private const string MissingTitle = "Title n/a";
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
            column.Title = GetPropertyName(column.Property) ?? ColumnManager<TGridItem>.MissingTitle;

        Columns.Add(column);

        column.Id = Columns.Count;
    }

    public void AddSimple(Expression<Func<TGridItem, object?>> expression, string? title = null)
    {
        Add(new() { Property = expression, Title = title });
    }

    public void AddAction(Expression<Func<TGridItem, object?>> expression, string? title = null, Align align = Align.Left, string? @class = null, Func<TGridItem, Task>? onClick = null)
    {
        var compiledExpression = expression.Compile();

        Add(new()
        {
            Title = string.IsNullOrWhiteSpace(title) ? GetPropertyName(expression) : title,
            ChildContent = (item) => (builder) =>
            {
                var value = compiledExpression.Invoke(item);
                builder.OpenElement(0, "div");
                if (onClick is not null)
                {
                    builder.AddAttribute(1, "onclick", EventCallback.Factory.Create(this, () => onClick.Invoke(item)));
                }
                builder.AddContent(2, value);
                builder.CloseElement();
            },
            SortBy = GridSort<TGridItem>.ByAscending(p => p == null ? default : compiledExpression.Invoke(p)),
            ColumnType = typeof(TemplateColumn<TGridItem>),
            Align = align,
            Class = @class
        });
    }

    public void AddAction(string staticContent, string? title = null, Align align = Align.Left, string? @class = null, Func<TGridItem, Task>? onClick = null)
    {
        Add(new()
        {
            Title = title ?? "Action",
            ChildContent = (TGridItem item) => (builder) =>
            {
                builder.OpenElement(0, "div");
                if (onClick != null)
                {
                    builder.AddAttribute(1, "onclick", EventCallback.Factory.Create(this, () => onClick.Invoke(item)));
                }

                builder.AddContent(2, staticContent);
                builder.CloseElement();
            },
            ColumnType = typeof(TemplateColumn<TGridItem>),
            Align = align,
            Class = @class
        });
    }

    public void AddSimpleNumber(Expression<Func<TGridItem, object?>> expression, string? title = null, string format = "N0")
    {
        var compiledExpression = expression.Compile();

        Add(new()
        {
            Title = string.IsNullOrWhiteSpace(title) ? GetPropertyName(expression) : title,
            ChildContent = (item) => (builder) =>
            {
                var value = compiledExpression.Invoke(item);
                if (value is decimal number)
                {
                    builder.AddContent(0, number.ToString(format));
                }
                else
                {
                    builder.AddContent(0, default(string));
                }
            },
            SortBy = GridSort<TGridItem>.ByAscending(p => p == null ? default : compiledExpression.Invoke(p)),
            ColumnType = typeof(TemplateColumn<TGridItem>),
            Format = format
        });
    }

    public void AddSimpleNumber2(Expression<Func<TGridItem, object?>> expression, string? title = null)
    {
        Add(new() { Property = expression, Title = title, Format = "N0", Align = Align.Right });
    }

    public void AddDoubleNumber(Expression<Func<TGridItem, object?>> expression, string? title = null, string format = "N0")
    {
        var compiledExpression = expression.Compile();

        Add(new()
        {
            Title = string.IsNullOrWhiteSpace(title) ? GetPropertyName(expression) : title,
            ChildContent = (item) => (builder) =>
            {
                var value = compiledExpression.Invoke(item);
                if (value is double number)
                {
                    builder.AddContent(0, number.ToString(format));
                }
                else
                {
                    builder.AddContent(0, default(string));
                }
            },
            SortBy = GridSort<TGridItem>.ByAscending(p => p == null ? default : compiledExpression.Invoke(p)),
            ColumnType = typeof(TemplateColumn<TGridItem>),
            Format = format
        });
    }

    public void AddSimpleDate(Expression<Func<TGridItem, object?>> expression, string? title = null, string format = "dd/MM/yyyy")
    {
        var compiledExpression = expression.Compile();

        Add(new()
        {
            Title = string.IsNullOrWhiteSpace(title) ? GetPropertyName(expression) : title,
            ChildContent = (item) => (builder) =>
            {
                var value = compiledExpression.Invoke(item);
                if (value is DateTime date)
                {
                    builder.AddContent(0, date.ToString(format));
                }
                else
                {
                    builder.AddContent(0, default(string));
                }
            },
            SortBy = GridSort<TGridItem>.ByAscending(p => p == null ? default : compiledExpression.Invoke(p)),
            ColumnType = typeof(TemplateColumn<TGridItem>),
            Format = format
        });
    }

    public void AddTickColumn(Expression<Func<TGridItem, object?>> expression, string? title = null, Align align = Align.Center)
    {
        Add(new() { Property = expression, ColumnType = typeof(TickColumn<TGridItem>), Title = title, Align = align });
    }

    public void AddImageColumn(Expression<Func<TGridItem, object?>> expression, string? title = null, Align align = Align.Center, string? @class = null)
    {
        Add(new() { Property = expression, ColumnType = typeof(ImageColumn<TGridItem>), Title = title, Align = align, Class = @class });
    }

    public void AddTemplateColumn(RenderFragment<TGridItem> childContent, string? title = null, Align align = Align.Center, GridSort<TGridItem>? sortBy = null, string? cssClass = null)
    {
        Add(new() { ChildContent = childContent, ColumnType = typeof(TemplateColumn<TGridItem>), Title = title, Align = align, SortBy = sortBy, Class = cssClass });
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