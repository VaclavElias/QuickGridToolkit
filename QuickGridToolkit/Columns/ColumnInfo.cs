namespace QuickGridToolkit.Columns;

public class ColumnInfo
{
    public string? Title { get; set; }
    public string? FullTitle { get; set; }
    public string? Class { get; set; }

    public ColumnInfo(string? title, string? fullTitle, string? @class)
    {
        Title = title;
        FullTitle = fullTitle;
        Class = @class;
    }
}