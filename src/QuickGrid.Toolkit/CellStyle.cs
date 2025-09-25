namespace QuickGrid.Toolkit;

/// <summary>
/// Represents a cell style mapping for a specific value.
/// </summary>
/// <typeparam name="TValue">The type of the value to style</typeparam>
/// <param name="Value">The value to match</param>
/// <param name="Style">The style to apply</param>
public record CellStyle<TValue>(TValue Value, string Style);