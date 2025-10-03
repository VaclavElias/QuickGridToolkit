using System.Dynamic;

namespace QuickGrid.Toolkit.Core;

public static class ExpandoObjectBuilder<TGridItem>
{
    /// <summary>
    /// Creates an ExpandoObject with properties extracted from an item based on the provided column names.
    /// </summary>
    /// <param name="item">The item to extract properties from.</param>
    /// <param name="columnNames">List of property names to extract.</param>
    /// <returns>An ExpandoObject with the extracted properties, or null if the item is null.</returns>
    public static IDictionary<string, object?>? Create(TGridItem? item, List<string> columnNames)
    {
        if (item is null) return null;

        var obj = new ExpandoObject() as IDictionary<string, object?>;

        foreach (var column in columnNames)
        {
            if (column is null) continue;

            if (IsNestedProperty(column))
            {
                ExpandoObjectBuilder<TGridItem>.ExtractNestedProperty(obj, item, column);
            }
            else
            {
                ExpandoObjectBuilder<TGridItem>.ExtractSimpleProperty(obj, item, column);
            }
        }

        return obj;
    }

    /// <summary>
    /// Checks if a property name represents a nested property (contains a dot).
    /// </summary>
    private static bool IsNestedProperty(string propertyName) => propertyName.Contains('.');

    /// <summary>
    /// Extracts a nested property value from an item and adds it to the provided dictionary.
    /// </summary>
    private static void ExtractNestedProperty(IDictionary<string, object?> dict, TGridItem item, string propertyPath)
    {
        var propertyParts = propertyPath.Split('.');

        if (propertyParts.Length < 2) return;

        var rootProperty = item!.GetType().GetProperty(propertyParts[0]);

        if (rootProperty is null) return;

        var rootValue = rootProperty.GetValue(item);

        if (rootValue is null) return;

        var childProperty1 = rootValue.GetType().GetProperty(propertyParts[1]);

        if (childProperty1 is null) return;

        var childValue1 = childProperty1.GetValue(rootValue);

        if (childValue1 is null) return;

        if (propertyParts.Length > 2)
        {
            var childProperty2 = childValue1.GetType().GetProperty(propertyParts[2]);

            if (childProperty2 is null) return;

            var childValue2 = childProperty2.GetValue(childValue1);

            dict.Add(propertyPath, childValue2);

            return;
        }

        dict.Add(propertyPath, childValue1);
    }

    /// <summary>
    /// Extracts a simple property value from an item and adds it to the provided dictionary.
    /// </summary>
    private static void ExtractSimpleProperty(IDictionary<string, object?> dict, TGridItem item, string propertyName)
    {
        var property = item!.GetType().GetProperty(propertyName);

        if (property is null) return;

        var value = property.GetValue(item);

        dict.Add(propertyName, value);
    }
}