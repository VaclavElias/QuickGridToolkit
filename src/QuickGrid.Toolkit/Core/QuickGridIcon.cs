namespace QuickGrid.Toolkit.Core;

public enum QuickGridIcon
{
    ColumnLayout,
    Settings,
    Filter,
    Search,
    ClearSearch,
    Export,
    Tick,
    ExportSelected,
    Wrench
}

public class DefaultQuickGridIconProvider : IQuickGridIconProvider
{
    public RenderFragment Render(QuickGridIcon icon, string? extraCss = null) => icon switch
    {
        QuickGridIcon.ColumnLayout => builder
            => builder.AddMarkupContent(0, "<i class=\"bi bi-window-sidebar\"></i>"),
        QuickGridIcon.Settings => builder
            => builder.AddMarkupContent(0, "<i class=\"bi bi-gear\"></i>"),
        QuickGridIcon.Filter => builder
            => builder.AddMarkupContent(0, "<i class=\"bi bi-funnel\"></i>"),
        QuickGridIcon.Export => builder
            => builder.AddMarkupContent(0, "<i class=\"bi bi-download me-2\"></i>"),
        QuickGridIcon.ExportSelected => builder
            => builder.AddMarkupContent(0, "<i class=\"bi bi-download me-2\"></i>"),
        QuickGridIcon.Tick => builder
            => builder.AddMarkupContent(0, "<i class=\"bi bi-check-lg me-2 text-success\"></i>"),
        QuickGridIcon.Wrench => builder
            => builder.AddMarkupContent(0, "<i class=\"bi bi-wrench me-2\"></i>"),
        _ => throw new NotImplementedException()
    };
}

public class FontAwesomeQuickGridIconProvider : IQuickGridIconProvider
{
    public RenderFragment Render(QuickGridIcon icon, string? extraCss = null) => icon switch
    {
        QuickGridIcon.ColumnLayout => builder
            => builder.AddMarkupContent(0, "<i class=\"fal fa-table-layout\"></i>"),
        QuickGridIcon.Settings => builder
            => builder.AddMarkupContent(0, "<i class=\"fal fa-cog\"></i>"),
        QuickGridIcon.Filter => builder
            => builder.AddMarkupContent(0, "<i class=\"fal fa-filters\"></i>"),
        QuickGridIcon.Export => builder
            => builder.AddMarkupContent(0, "<i class=\"fal fa-file-export fa-fw\"></i>"),
        QuickGridIcon.ExportSelected => builder
            => builder.AddMarkupContent(0, "<i class=\"fal fa-file-export fa-fw\"></i>"),
        QuickGridIcon.Tick => builder
            => builder.AddMarkupContent(0, "<i class=\"far fa-check fa-fw text-success\"></i>"),
        QuickGridIcon.Wrench => builder
            => builder.AddMarkupContent(0, "<i class=\"fal fa-wrench fa-fw\"></i>"),
        _ => throw new NotImplementedException()
    };
}