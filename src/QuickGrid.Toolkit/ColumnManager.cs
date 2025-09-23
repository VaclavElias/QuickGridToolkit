using System.Globalization;

namespace QuickGrid.Toolkit;

/// <summary>
/// Represents a cell style mapping for a specific value.
/// </summary>
/// <typeparam name="TValue">The type of the value to style</typeparam>
/// <param name="Value">The value to match</param>
/// <param name="Style">The style to apply</param>
public record CellStyle<TValue>(TValue Value, string Style);

/// <summary>
/// Provides styling mappings for cell values, including support for null values.
/// </summary>
/// <typeparam name="TValue">The type of values to map</typeparam>
public class CellStyleMap<TValue>
{
    private readonly List<CellStyle<TValue>> _styleMappings = [];
    private string? _nullValueStyle;

    /// <summary>
    /// Adds a style mapping for a specific value.
    /// </summary>
    /// <param name="value">The value to map</param>
    /// <param name="style">The style to apply</param>
    /// <returns>This instance for method chaining</returns>
    public CellStyleMap<TValue> Add(TValue value, string style)
    {
        if (value is null)
        {
            _nullValueStyle = style;
        }
        else
        {
            _styleMappings.Add(new CellStyle<TValue>(value, style));
        }
        return this;
    }

    /// <summary>
    /// Adds multiple style mappings.
    /// </summary>
    /// <param name="mappings">The mappings to add</param>
    /// <returns>This instance for method chaining</returns>
    public CellStyleMap<TValue> AddRange(IEnumerable<CellStyle<TValue>> mappings)
    {
        _styleMappings.AddRange(mappings);
        return this;
    }

    /// <summary>
    /// Sets the style to use for null values.
    /// </summary>
    /// <param name="style">The style to apply to null values</param>
    /// <returns>This instance for method chaining</returns>
    public CellStyleMap<TValue> SetNullStyle(string style)
    {
        _nullValueStyle = style;
        return this;
    }

    /// <summary>
    /// Gets the style for a given value.
    /// </summary>
    /// <param name="value">The value to get the style for</param>
    /// <returns>The style string, or empty string if no mapping exists</returns>
    public string GetStyle(TValue? value)
    {
        if (value is null)
        {
            return _nullValueStyle ?? string.Empty;
        }

        var mapping = _styleMappings.FirstOrDefault(m => EqualityComparer<TValue>.Default.Equals(m.Value, value));
        return mapping?.Style ?? string.Empty;
    }

    /// <summary>
    /// Checks if a mapping exists for the given value.
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if a mapping exists</returns>
    public bool ContainsValue(TValue? value)
    {
        if (value is null)
        {
            return _nullValueStyle is not null;
        }

        return _styleMappings.Any(m => EqualityComparer<TValue>.Default.Equals(m.Value, value));
    }

    /// <summary>
    /// Creates a CellStyleMap from a collection of CellStyle mappings.
    /// </summary>
    /// <param name="mappings">The style mappings</param>
    /// <param name="nullValueStyle">Optional style for null values</param>
    /// <returns>A new CellStyleMap instance</returns>
    public static CellStyleMap<TValue> FromMappings(IEnumerable<CellStyle<TValue>> mappings, string? nullValueStyle = null)
    {
        var map = new CellStyleMap<TValue>();
        map.AddRange(mappings);

        if (nullValueStyle is not null)
        {
            map.SetNullStyle(nullValueStyle);
        }

        return map;
    }

    /// <summary>
    /// Clears all mappings. Optionally clears the null value style.
    /// </summary>
    /// <param name="clearNullStyle">When true, also clears the null value style.</param>
    /// <returns>This instance for method chaining</returns>
    public CellStyleMap<TValue> Clear(bool clearNullStyle = true)
    {
        _styleMappings.Clear();
        if (clearNullStyle) _nullValueStyle = null;
        return this;
    }

    /// <summary>
    /// Replaces all mappings (and optional null style) in one step.
    /// </summary>
    /// <param name="mappings">New mappings</param>
    /// <param name="nullValueStyle">Optional style for null values</param>
    /// <returns>This instance for method chaining</returns>
    public CellStyleMap<TValue> ReplaceWith(IEnumerable<CellStyle<TValue>> mappings, string? nullValueStyle = null)
    {
        _styleMappings.Clear();
        _styleMappings.AddRange(mappings);
        _nullValueStyle = nullValueStyle;
        return this;
    }

    /// <summary>
    /// Removes the mapping for the given value. When value is null, clears the null style.
    /// </summary>
    public bool Remove(TValue? value)
    {
        if (value is null)
        {
            var hadNull = _nullValueStyle is not null;
            _nullValueStyle = null;
            return hadNull;
        }

        var idx = _styleMappings.FindIndex(m => EqualityComparer<TValue>.Default.Equals(m.Value, value));
        if (idx >= 0)
        {
            _styleMappings.RemoveAt(idx);
            return true;
        }
        return false;
    }
}

public class ColumnManager<TGridItem>
{
    private const string MissingTitle = "Title n/a";
    private const string NegativeDescription = "negative";
    private const string PositiveDescription = "positive";
    private const string ZeroDescription = "zero";
    private const string UnknownDescription = "unknown";
    private const string NoValueDescription = "no-value";

    public bool IsIndexColumn { get; set; } = true;
    public List<DynamicColumn<TGridItem>> Columns { get; } = [];
    public List<FooterColumn<IEnumerable<TGridItem>>> FooterColumns { get; } = [];

    /// <summary>
    /// Returns visible columns
    /// </summary>
    /// <returns></returns>
    public IEnumerable<DynamicColumn<TGridItem>> Get() => Columns.Where(w => w.Visible);

    public void Add(DynamicColumn<TGridItem>? column = default)
    {
        if (column == null) return;

        if (string.IsNullOrWhiteSpace(column.Title))
            column.Title = GetPropertyName(column.Property) ?? MissingTitle;

        if (string.IsNullOrEmpty(column.PropertyName))
            column.PropertyName = GetPropertyNameNew(column.Property);

        Columns.Add(column);

        column.Id = Columns.Count;
    }

    public void AddSimple<TValue>(
        Expression<Func<TGridItem, TValue?>> expression,
        ColumnInfo columnInfo,
        string? format = null,
        Align align = Align.Left,
        CellStyleMap<TValue>? cellStyle = null,
        GridSort<TGridItem>? sortBy = null,
        bool visible = true,
        string? propertyName = null)
    {
        AddSimple(expression, columnInfo.Title, columnInfo.FullTitle, format, columnInfo.Class, align, cellStyle, sortBy, visible, propertyName);
    }

    /// <summary>
    /// Adds a simple date column to the grid based on a specified expression.
    /// /// </summary>
    /// <param name="expression">An expression to determine the property of the grid item to display.</param>
    /// <param name="title">The title of the column. If null or whitespace, the property name is used.</param>
    /// <param name="format">The date format string. Defaults to 'dd/MM/yyyy'.</param>
    public void AddSimple<TValue>(
        Expression<Func<TGridItem, TValue?>> expression,
        string? title = null,
        string? fullTitle = null,
        string? format = null,
        string? @class = null,
        Align align = Align.Left,
        CellStyleMap<TValue>? cellStyle = null,
        GridSort<TGridItem>? sortBy = null,
        bool visible = true,
        string? propertyName = null,
        bool? addToContent = null)
    {
        DynamicColumn<TGridItem> column = BuildColumn(expression, title, fullTitle, @class, align, sortBy);

        column.ChildContent = (item) => (builder) =>
        {
            if (item == null) return;

            var value = expression.Compile().Invoke(item);

            if (value is null)
            {
                builder.AddContent(0, string.Empty);
            }
            else
            {
                string displayValue;

                if (value is IFormattable formattableValue)
                    displayValue = formattableValue.ToString(format, CultureInfo.InvariantCulture);
                else
                    displayValue = $"{value}";

                if (addToContent == true)
                {
                    builder.AddMarkupContent(0, $"<span content=\"{value}\">{displayValue}</span>");
                }
                else if (cellStyle != null)
                {
                    builder.AddMarkupContent(0, $"<span content=\"{cellStyle.GetStyle(value)}\">{displayValue}</span>");
                }
                else
                {
                    builder.AddContent(0, displayValue);
                }
            }
        };

        column.Visible = visible;
        column.PropertyName = propertyName;

        Add(column);
    }

    // ToDo: This will replace AddSimpleDate
    public void AddSimpleDate2<TValue>(
        Expression<Func<TGridItem, TValue?>> expression,
        string? title = null,
        string? fullTitle = null,
        string? format = "dd/MM/yyyy",
        string? @class = null,
        Align align = Align.Center,
        CellStyleMap<TValue>? cellStyle = null,
        bool visible = true)
            => AddSimple(expression, title, fullTitle, format, @class, align, cellStyle, visible: visible);

    public void AddAction(Expression<Func<TGridItem, object?>> expression, ColumnInfo columnInfo, Align align = Align.Left, GridSort<TGridItem>? sortBy = null,
        bool visible = true, Func<TGridItem, Task>? onClick = null)
    {
        AddAction(expression, columnInfo.Title, columnInfo.FullTitle, align, columnInfo.Class, sortBy, visible, onClick, columnInfo.PropertyName);
    }

    public void AddAction(Expression<Func<TGridItem, object?>> expression, string? title = null, string? fullTitle = null, Align align = Align.Left, string? @class = null, GridSort<TGridItem>? sortBy = null,
        bool visible = true, Func<TGridItem, Task>? onClick = null, string? propertyName = null)
    {
        var compiledExpression = expression.Compile();

        Add(new()
        {
            Title = string.IsNullOrWhiteSpace(title) ? GetPropertyName(expression) : title,
            FullTitle = fullTitle,
            ChildContent = (item) => (builder) =>
            {
                var value = compiledExpression.Invoke(item);
                builder.OpenElement(0, "div");
                if (onClick is not null)
                    builder.AddAttribute(1, "onclick", EventCallback.Factory.Create(this, () => onClick.Invoke(item)));
                builder.AddContent(2, value);
                builder.CloseElement();
            },
            SortBy = sortBy ?? GridSort<TGridItem>.ByAscending(p => p == null ? default : compiledExpression.Invoke(p)),
            ColumnType = typeof(TemplateColumn<TGridItem>),
            Align = align,
            Class = @class,
            Visible = visible,
            PropertyName = propertyName
        });
    }

    public void AddAction(
        string staticContent,
        string? title = null,
        Align align = Align.Left,
        string? @class = null,
        Func<TGridItem, Task>? onClick = null)
    {
        Add(new()
        {
            Title = title ?? "Action",
            ChildContent = (item) => (builder) =>
            {
                builder.OpenElement(0, "div");
                if (onClick != null)
                    builder.AddAttribute(1, "onclick", EventCallback.Factory.Create(this, () => onClick.Invoke(item)));
                builder.AddContent(2, staticContent);
                builder.CloseElement();
            },
            ColumnType = typeof(TemplateColumn<TGridItem>),
            Align = align,
            Class = @class
        });
    }

    public void AddAction(
        string staticContent,
        string? title = null,
        Align align = Align.Left,
        string? @class = null,
        Action<TGridItem>? onClick = null,
        Expression<Func<TGridItem, bool>>? enabled = null)
    {
        Add(new()
        {
            Title = title ?? "Action",
            ChildContent = (item) => (builder) =>
            {
                if (enabled?.Compile().Invoke(item) != false)
                {
                    builder.OpenElement(0, "div");
                    if (onClick != null)
                        builder.AddAttribute(1, "onclick", EventCallback.Factory.Create(this, () => onClick.Invoke(item)));
                    builder.AddContent(2, staticContent);
                    builder.CloseElement();
                }
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
                    builder.AddContent(0, number.ToString(format));
                else
                    builder.AddContent(0, default(string));
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
    public void AddNumber(Expression<Func<TGridItem, decimal?>> expression, string? title = null, string? fullTitle = null, string format = "N0", string? @class = null, Align align = Align.Right, bool visible = true)
    {
        DynamicColumn<TGridItem> column = BuildColumn(expression, title, fullTitle, @class, align, visible: visible);

        column.ChildContent = (item) => (builder) =>
        {
            if (item == null) return;

            var value = expression.Compile().Invoke(item);

            builder.AddContent(0, value?.ToString(format));
        };

        column.IsNumeric = true;

        Add(column);
    }

    /// <summary>
    /// Adds a double numeric column to the grid.
    /// </summary>
    public void AddNumber(Expression<Func<TGridItem, double?>> expression, string? title = null, string? fullTitle = null, string format = "N0", string? @class = null, Align align = Align.Right, bool visible = true)
    {
        DynamicColumn<TGridItem> column = BuildColumn(expression, title, fullTitle, @class, align, visible: visible);

        column.ChildContent = (item) => (builder) =>
        {
            if (item == null) return;

            var value = expression.Compile().Invoke(item);

            builder.AddContent(0, value?.ToString(format));
        };

        column.IsNumeric = true;

        Add(column);
    }

    /// <summary>
    /// Adds a int numeric column to the grid.
    /// </summary>
    public void AddNumber(Expression<Func<TGridItem, int?>> expression, string? title = null, string? fullTitle = null, string format = "N0", string? @class = null, Align align = Align.Right, string? propertyName = null)
    {
        DynamicColumn<TGridItem> column = BuildColumn(expression, title, fullTitle, @class, align);

        column.ChildContent = (item) => (builder) =>
        {
            if (item == null) return;

            var value = expression.Compile().Invoke(item);

            builder.AddContent(0, value?.ToString(format));
        };

        column.IsNumeric = true;
        column.PropertyName = propertyName;

        Add(column);
    }

    public void AddStyledNumber<TValue>(
        Expression<Func<TGridItem, TValue?>> expression,
        ColumnInfo columnInfo,
        string format = "N0",
        Align align = Align.Right,
        bool visible = true,
        CellStyleMap<TValue>? cellStyle = null,
        Func<TGridItem, Task>? onClick = null) where TValue : struct, IFormattable
        => AddStyledNumber(expression, columnInfo.Title, columnInfo.FullTitle, format, columnInfo.Class, align, visible, cellStyle, onClick, columnInfo.PropertyName);

    public void AddStyledNumber<TValue>(
        Expression<Func<TGridItem, TValue?>> expression,
        string? title = null,
        string? fullTitle = null,
        string format = "N0",
        string? @class = null,
        Align align = Align.Right,
        bool visible = true,
        CellStyleMap<TValue>? cellStyle = null,
        Func<TGridItem, Task>? onClick = null,
        string? propertyName = null) where TValue : struct, IFormattable
    {
        DynamicColumn<TGridItem> column = BuildColumn(expression, title, fullTitle, @class, align);

        column.ChildContent = (item) => (builder) =>
        {
            if (item == null) return;

            var value = expression.Compile().Invoke(item);

            if (value.HasValue)
            {
                string formattedValue = value.Value.ToString(format, CultureInfo.InvariantCulture);
                string content = $"<span content=\"{DetermineNumericValueNature(value.Value, cellStyle)}\">{formattedValue}</span>";

                if (onClick is null)
                    builder.AddMarkupContent(0, content);
                else
                {
                    builder.OpenElement(0, "div");
                    builder.AddAttribute(1, "onclick", EventCallback.Factory.Create(this, () => onClick.Invoke(item)));
                    builder.AddMarkupContent(2, content);
                    builder.CloseElement();
                }
            }
            else
            {
                // Handle null case with cellStyle
                var nullStyle = cellStyle?.GetStyle(default(TValue)) ?? string.Empty;
                if (!string.IsNullOrEmpty(nullStyle))
                {
                    builder.AddMarkupContent(0, $"<span content=\"{nullStyle}\"></span>");
                }
                else
                {
                    builder.AddContent(0, string.Empty);
                }
            }
        };

        column.Visible = visible;
        column.IsNumeric = true;
        column.PropertyName = propertyName;

        Add(column);
    }

    private static DynamicColumn<TGridItem> BuildColumn<TValue>(Expression<Func<TGridItem, TValue?>> expression, string? title, string? fullTitle = null, string? @class = null, Align align = Align.Left, GridSort<TGridItem>? sortBy = null, bool visible = true) => new()
    {
        Title = title ?? GetPropertyName(expression),
        SortBy = sortBy ?? GridSort<TGridItem>.ByAscending(expression),
        ColumnType = typeof(TemplateColumn<TGridItem>),
        Align = align,
        FullTitle = fullTitle,
        Class = @class,
        Visible = visible,
        Property = ConvertExpressionToObject(expression)
    };

    private static Expression<Func<TGridItem, object?>> ConvertExpressionToObject<TValue>(
        Expression<Func<TGridItem, TValue?>> expression)
    {
        // Check if the body's result type is already object to avoid unnecessary conversion
        if (typeof(TValue) == typeof(object))
            // Safe to return as is because TValue is object. However, we need to handle nullable conversion.
            return expression as Expression<Func<TGridItem, object?>>
                   ?? throw new InvalidOperationException("Failed to convert expression.");

        // Prepare a conversion of the expression's body to object?
        var body = expression.Body;

        // Handle nullable value types by converting to object
        if (typeof(TValue).IsValueType)
            body = Expression.Convert(body, typeof(object));

        // Rebuild the lambda expression with the converted body
        var convertedExpression = Expression.Lambda<Func<TGridItem, object?>>(body, expression.Parameters);

        return convertedExpression;
    }

    private static string DetermineNumericValueNature<TValue>(TValue? value, CellStyleMap<TValue>? cellStyle = null) where TValue : struct
    {
        // First check for custom styling
        if (cellStyle != null)
        {
            if (value.HasValue && cellStyle.ContainsValue(value.Value))
            {
                return cellStyle.GetStyle(value.Value);
            }
            if (!value.HasValue && cellStyle.ContainsValue(default(TValue)))
            {
                return cellStyle.GetStyle(default(TValue));
            }
        }

        // Default numeric value nature determination
        switch (value)
        {
            case null:
                return NoValueDescription;
            case int intValue when intValue < 0:
            case decimal decimalValue when decimalValue < 0:
            case double doubleValue when doubleValue < 0:
                return NegativeDescription;
            case int intValue when intValue > 0:
            case decimal decimalValue when decimalValue > 0:
            case double doubleValue when doubleValue > 0:
                return PositiveDescription;
            case int intValue when intValue == 0:
            case decimal decimalValue when decimalValue == 0:
            case double doubleValue when doubleValue == 0:
                return ZeroDescription;
            default:
                return UnknownDescription;
        }
    }

    private static string DetermineValueStyling<TValue>(TValue? value, CellStyleMap<TValue>? cellStyle = null)
    {
        return cellStyle?.GetStyle(value) ?? string.Empty;
    }

    public void AddTickColumn(
        Expression<Func<TGridItem, object?>> expression,
        string? title = null,
        string? fullTitle = null,
        string? @class = null,
        Align align = Align.Center,
        bool showOnlyTrue = false)
    {
        var column = new TickPropertyColumn<TGridItem>()
        {
            Property = expression,
            Title = title,
            FullTitle = fullTitle,
            ColumnType = typeof(TickColumn<TGridItem>),
            Align = align,
            ShowOnlyTrue = showOnlyTrue,
            Class = @class,
        };

        Add(column);
    }

    public void AddToggleColumn(
        Expression<Func<TGridItem, object?>> expression,
        string? title = null,
        string? fullTitle = null,
        string? @class = "text-center",
        Align align = Align.Center,
        Func<TGridItem, Task>? onChange = null)
    {

        Add(new()
        {
            Property = expression,
            Title = title,
            FullTitle = fullTitle,
            ColumnType = typeof(ToggleColumn<TGridItem>),
            Align = align,
            Class = @class,
            OnActionAsync = onChange
        });
    }

    public void AddImageColumn(Expression<Func<TGridItem, object?>> expression, string? title = null, Align align = Align.Center, string? @class = null)
    {
        Add(new() { Property = expression, ColumnType = typeof(ImageColumn<TGridItem>), Title = title, Align = align, Class = @class });
    }

    public void AddTemplateColumn(RenderFragment<TGridItem> childContent, string? title = null, string? fullTitle = null, Align align = Align.Center, GridSort<TGridItem>? sortBy = null, string? cssClass = null)
    {
        Add(new() { ChildContent = childContent, ColumnType = typeof(TemplateColumn<TGridItem>), Title = title, Align = align, FullTitle = fullTitle, SortBy = sortBy, Class = cssClass });
    }

    public void AddIndexColumn(string title = "#", Align align = Align.Center)
        => Add(new() { ColumnType = typeof(EmptyColumn<TGridItem>), Title = title, Align = align });

    /// <summary>
    /// Creates a shallow copy of the current list of <see cref="DynamicColumn{TGridItem}"/> objects,
    /// cloning only basic properties such as Title, FullTitle, Property, ColumnType, Format, and Visibility.
    /// </summary>
    /// <remarks>
    /// This method performs a shallow copy, meaning that only the values of the properties are copied.
    /// Any modifications to the properties of the cloned objects will not affect the original objects in the list.
    /// </remarks>
    /// <returns>A new list containing cloned instances of <see cref="DynamicColumn{TGridItem}"/>
    /// where each column retains the basic property values of the corresponding original column.</returns>
    public List<DynamicColumn<TGridItem>> SimpleClone()
    {
        return Columns.ConvertAll(s => new DynamicColumn<TGridItem>
        {
            Id = s.Id,
            Title = s.Title,
            FullTitle = s.FullTitle,
            Property = s.Property,
            ColumnType = s.ColumnType,
            Format = s.Format,
            Visible = s.Visible
        });
    }

    public void AddFooterColumn(
        int id,
        object? value,
        string? format = null,
        string? @class = null,
        Align align = Align.Left,
        bool visible = true)
    {
        FooterColumn<IEnumerable<TGridItem>> column = new()
        {
            Id = id,
            Format = format,
            Class = @class,
            Align = align,
            Visible = visible,
        };

        var displayValue = BuildDisplayValue(value, format);

        column.Content = $"<td class=\"{column.Class}\">{displayValue}</td>";

        FooterColumns.Add(column);
    }

    public void AddFooterColumn<TValue>(
        int id,
        Expression<Func<IEnumerable<TGridItem>, TValue?>> expression,
        string? format = null,
        string? @class = null,
        Align align = Align.Left,
        bool visible = true)
    {
        FooterColumn<IEnumerable<TGridItem>> column = new()
        {
            Id = id,
            Format = format,
            Class = @class,
            Align = align,
            Visible = visible,
        };

        var compiledExpression = expression.Compile();

        column.StringContent = (item) =>
        {
            if (item == null) return null;

            var value = compiledExpression.Invoke(item);

            var displayValue = BuildDisplayValue(value, format);

            return $"<td class=\"{column.Class}\">{displayValue}</td>";
        };

        FooterColumns.Add(column);
    }

    public void AddFooterColumnWithSum(DynamicColumn<TGridItem> column, string removeClass = "")
    {
        var compiledExpression = column.Property!.Compile();

        AddFooterColumn(
            column.Id,
            items => items.Sum(item => Convert.ToDecimal(compiledExpression(item))),
            format: "N0",
            @class: column.Class?.Replace(removeClass, "")
        );
    }

    private static string BuildDisplayValue(object? value, string? format)
    {
        if (value is null)
            return string.Empty;
        else if (value is IFormattable formattableValue)
            return formattableValue.ToString(format, CultureInfo.InvariantCulture);
        else
            return $"{value}";
    }

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

    private static string? GetPropertyNameNew<TValue>(Expression<Func<TGridItem, TValue?>>? expression)
    {
        if (expression is null) return null;

        Expression body = expression.Body;

        while (body is UnaryExpression unaryExpression1)
            body = unaryExpression1.Operand;

        if (body is MemberExpression memberExpression1 && memberExpression1.Member is PropertyInfo propertyInfo1)
            return propertyInfo1.Name;

        // Handle more complex expressions by extracting property references
        var propertyVisitor = new PropertyReferenceVisitor();

        propertyVisitor.Visit(expression);

        if (propertyVisitor.PropertyNames.Count > 0)
            return string.Join("_", propertyVisitor.PropertyNames);

        // If we couldn't extract any property, generate a safe name based on the expression
        return $"Expr_{Math.Abs(expression.ToString().GetHashCode())}";
    }

    // Helper class to extract property names from expressions
    private class PropertyReferenceVisitor : ExpressionVisitor
    {
        public List<string> PropertyNames { get; } = [];

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member is PropertyInfo propertyInfo)
                PropertyNames.Add(propertyInfo.Name);

            return base.VisitMember(node);
        }
    }
}