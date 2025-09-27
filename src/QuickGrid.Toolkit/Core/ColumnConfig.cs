namespace QuickGrid.Toolkit.Core;

public class ColumnConfig
{
    /// <summary>
    /// Unique identifier for each saved configuration.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the configuration, such as "Default", "Detailed View", etc.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Timestamp when the configuration was created.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// List of selected columns in this configuration.
    /// </summary>
    public List<SelectedColumn> ColumnSelections { get; set; } = [];

    public required string TableId { get; set; }
    public bool Shared { get; set; }
    public bool Default { get; set; }
    public bool IsDeleted { get; set; }

    public bool IsColumnSelected(string? columnName)
        => ColumnSelections.Any(x => x.ColumnName == columnName);
}