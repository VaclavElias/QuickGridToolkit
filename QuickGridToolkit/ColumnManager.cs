using System.Globalization;

namespace QuickGridToolkit;

public class ColumnManager<TGridItem>
{
    private const string MissingTitle = "Title n/a";
    private const string NegativeDescription = "negative";
    private const string PositiveDescription = "positive";
    private const string ZeroDescription = "zero";
    private const string UnknownDescription = "unknown";
    private const string NoValueDescription = "no-value";
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

    [Obsolete("Use AddNumber instead.", true)]
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

    [Obsolete("Use AddNumber instead.", true)]
    public void AddSimpleNumber2(Expression<Func<TGridItem, object?>> expression, string? title = null)
    {
        Add(new() { Property = expression, Title = title, Format = "N0", Align = Align.Right });
    }

    /// <summary>
    /// Adds a decimal numeric column to the grid.
    /// </summary>
    public void AddNumber(Expression<Func<TGridItem, decimal?>> expression, string? title = null, string? fullTitle = null, string format = "N0", string? @class = null)
    {
        DynamicColumn<TGridItem> column = BuildNumericColumn(expression, title, fullTitle, @class);

        column.ChildContent = (item) => (builder) =>
        {
            if (item == null) return;

            var value = expression.Compile().Invoke(item);

            builder.AddContent(0, value?.ToString(format));
        };

        Add(column);
    }

    /// <summary>
    /// Adds a double numeric column to the grid.
    /// </summary>
    public void AddNumber(Expression<Func<TGridItem, double?>> expression, string? title = null, string? fullTitle = null, string format = "N0", string? @class = null)
    {
        DynamicColumn<TGridItem> column = BuildNumericColumn(expression, title, fullTitle, @class);

        column.ChildContent = (item) => (builder) =>
        {
            if (item == null) return;

            var value = expression.Compile().Invoke(item);

            builder.AddContent(0, value?.ToString(format));
        };

        Add(column);
    }

    /// <summary>
    /// Adds a int numeric column to the grid.
    /// </summary>
    public void AddNumber(Expression<Func<TGridItem, int?>> expression, string? title = null, string? fullTitle = null, string format = "N0", string? @class = null)
    {
        DynamicColumn<TGridItem> column = BuildNumericColumn(expression, title, fullTitle, @class);

        column.ChildContent = (item) => (builder) =>
        {
            if (item == null) return;

            var value = expression.Compile().Invoke(item);

            builder.AddContent(0, value?.ToString(format));
        };

        Add(column);
    }

    public void AddStyledNumber<TValue>(
        Expression<Func<TGridItem, TValue?>> expression,
        string? title = null, string?
        fullTitle = null,
        string format = "N0",
        string? @class = null,
        Dictionary<TValue, string>? customStyling = null) where TValue : struct, IFormattable
    {
        DynamicColumn<TGridItem> column = BuildNumericColumn(expression, title, fullTitle, @class);

        column.ChildContent = (item) => (builder) =>
        {
            if (item == null) return;

            var value = expression.Compile().Invoke(item);

            if (value.HasValue)
            {
                string formattedValue = value.Value.ToString(format, CultureInfo.InvariantCulture);
                builder.AddMarkupContent(0, $"<span content=\"{DetermineValueNature(value.Value, customStyling)}\">{formattedValue}</span>");
            }
            else
            {
                builder.AddContent(0, string.Empty);
            }
        };

        Add(column);
    }

    private static DynamicColumn<TGridItem> BuildNumericColumn<TValue>(Expression<Func<TGridItem, TValue?>> expression, string? title, string? fullTitle = null, string? @class = null) => new DynamicColumn<TGridItem>()
    {
        Title = title ?? GetPropertyName(expression),
        SortBy = GridSort<TGridItem>.ByAscending(expression),
        ColumnType = typeof(TemplateColumn<TGridItem>),
        Align = Align.Right,
        FullTitle = fullTitle,
        Class = @class
    };

    private static string DetermineValueNature<TValue>(TValue? value, Dictionary<TValue, string>? customStyling = null) where TValue : struct
    {
        switch (value)
        {
            case TValue customValue when customStyling?.ContainsKey(customValue) == true:
                return customStyling[customValue];
            case null:
                return NoValueDescription;
            case int intValue when intValue < 0:
            case decimal decimalValue when decimalValue < 0:
                return NegativeDescription;
            case int intValue when intValue > 0:
            case decimal decimalValue when decimalValue > 0:
                return PositiveDescription;
            case int intValue when intValue == 0:
            case decimal decimalValue when decimalValue == 0:
                return ZeroDescription;
            default:
                return UnknownDescription;
        }
    }

    /// <summary>
    /// Adds a simple date column to the grid based on a specified expression.
    /// </summary>
    /// <param name="expression">An expression to determine the property of the grid item to display.</param>
    /// <param name="title">The title of the column. If null or whitespace, the property name is used.</param>
    /// <param name="format">The date format string. Defaults to 'dd/MM/yyyy'.</param>
    public void AddSimpleDate(Expression<Func<TGridItem, object?>> expression, string? title = null, string format = "dd/MM/yyyy")
    {
        var compiledExpression = expression.Compile();

        Add(new()
        {
            Title = string.IsNullOrWhiteSpace(title) ? GetPropertyName(expression) : title,
            ChildContent = (item) => (builder) =>
            {
                var value = compiledExpression.Invoke(item);
                var displayValue = value switch
                {
                    DateTime date => date.ToString(format),
                    DateOnly dateOnly => dateOnly.ToString(format),
                    _ => default
                };
                builder.AddContent(0, displayValue);
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

    private static string? GetPropertyName<TValue>(Expression<Func<TGridItem, TValue?>>? expression)
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