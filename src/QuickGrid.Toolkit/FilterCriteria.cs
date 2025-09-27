namespace QuickGrid.Toolkit;

public class FilterCriteria<TGridItem>
{
    public string SearchTerm { get; set; } = null!;

    private readonly Func<string, Expression<Func<TGridItem, bool>>> _expressionBuilder;

    public FilterCriteria(Func<string, Expression<Func<TGridItem, bool>>> expressionBuilder)
    {
        _expressionBuilder = expressionBuilder;
    }

    public Expression<Func<TGridItem, bool>> CreateExpression()
    {
        return _expressionBuilder(SearchTerm);
    }
}