namespace QuickGrid.Toolkit.Helpers;

/// <summary>
/// Provides utility methods for determining cell styling based on values.
/// </summary>
internal static class CellStyleHelper
{
    private const string NegativeDescription = "negative";
    private const string PositiveDescription = "positive";
    private const string ZeroDescription = "zero";
    private const string UnknownDescription = "unknown";
    private const string NoValueDescription = "no-value";

    /// <summary>
    /// Determines the style description for a numeric value.
    /// First checks for custom styling from CellStyleMap, then falls back to default numeric nature determination.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to determine styling for.</param>
    /// <param name="cellStyle">Optional custom cell style map.</param>
    /// <returns>A string describing the value's nature or custom style.</returns>
    public static string DetermineNumericValueNature<TValue>(TValue? value, CellStyleMap<TValue>? cellStyle = null) where TValue : struct
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
        return value switch
        {
            null => NoValueDescription,
            int intValue when intValue < 0 => NegativeDescription,
            decimal decimalValue when decimalValue < 0 => NegativeDescription,
            double doubleValue when doubleValue < 0 => NegativeDescription,
            int intValue when intValue > 0 => PositiveDescription,
            decimal decimalValue when decimalValue > 0 => PositiveDescription,
            double doubleValue when doubleValue > 0 => PositiveDescription,
            int intValue when intValue == 0 => ZeroDescription,
            decimal decimalValue when decimalValue == 0 => ZeroDescription,
            double doubleValue when doubleValue == 0 => ZeroDescription,
            _ => UnknownDescription
        };
    }

    /// <summary>
    /// Gets the style for a value from the provided cell style map.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to get styling for.</param>
    /// <param name="cellStyle">The cell style map to use.</param>
    /// <returns>The style string, or empty string if no style map is provided or no mapping exists.</returns>
    public static string GetValueStyle<TValue>(TValue? value, CellStyleMap<TValue>? cellStyle = null)
    {
        return cellStyle?.GetStyle(value) ?? string.Empty;
    }
}