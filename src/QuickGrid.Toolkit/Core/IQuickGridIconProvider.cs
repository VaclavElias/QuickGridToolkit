namespace QuickGrid.Toolkit.Core;

public interface IQuickGridIconProvider
{
    RenderFragment Render(QuickGridIcon icon, string? extraCss = null);
}