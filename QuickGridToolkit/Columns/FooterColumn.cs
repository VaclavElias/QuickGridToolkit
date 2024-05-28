namespace QuickGridToolkit.Columns;

public class FooterColumn<TGridItem>
{
    public int Id { get; set; }
    public string ColumnId => $"column-{Id}";
    public bool Visible { get; set; } = true;
    public Align Align { get; set; }
    public string? Format { get; set; }
    public string? Class { get; set; }
    public object? Value { get; set; }
    public Func<TGridItem, string?>? StringContent { get; set; }
}
