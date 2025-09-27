namespace QuickGrid.Toolkit.Core;

public class SelectedColumn
{
    /// <summary>
    /// Unique identifier for each selected column.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Links back to the ColumnConfiguration to associate columns with configurations.
    /// </summary>
    public int ColumnConfigurationId { get; set; }

    /// <summary>
    /// Name of the selected column in the table (e.g., "Name", "Status", "Date Created").
    /// </summary>
    public required string ColumnName { get; set; }

    /// <summary>
    /// Order in which the column should be displayed in the table.
    /// </summary>
    public int DisplayOrder { get; set; }

    ///// <summary>
    ///// Navigation property to the associated ColumnConfiguration.
    ///// </summary>
    //public ColumnConfig? ColumnConfiguration { get; set; }
}