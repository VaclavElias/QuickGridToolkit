using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Text;

namespace QuickGrid.Toolkit;

public partial class QuickGridWrapper<TGridItem> : ComponentBase, IDisposable
{
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Class { get; set; } = "table table-sm table-index table-striped small table-fit table-thead-sticky mb-0";
    [Parameter] public string? DownloadFileName { get; set; }
    [Parameter] public string? QuickSearch { get; set; }
    [Parameter] public IQueryable<TGridItem>? Items { get; set; }
    [Parameter] public ColumnManager<TGridItem> ColumnManager { get; set; } = new();
    [Parameter] public bool IsPaginator { get; set; }
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public bool IsSelection { get; set; }
    [Parameter] public bool IsColumnSelection { get; set; } = true;
    [Parameter] public bool IsColumnItemsSelection { get; set; }
    [Parameter] public bool IsFilter { get; set; } = true;
    [Parameter] public bool IsToolbar { get; set; } = true;
    [Parameter] public bool IsNestedSearch { get; set; } = true;
    [Parameter] public bool IsExportEnabled { get; set; }
    [Parameter] public Func<TGridItem, object> ItemKey { get; set; } = x => x!;
    [Parameter] public EventCallback ColumnSelectionChanged { get; set; }
    [Parameter] public EventCallback<List<TGridItem>> SearchResultChanged { get; set; }
    [Parameter] public QuickGridWrapperEvents<TGridItem>? Events { get; set; }
    /// <summary>
    /// The number of items to display per page when pagination is enabled. The default value is 20.
    /// </summary>
    [Parameter] public int ItemsPerPage { get; set; } = 20;
    [Parameter] public FilterCriteria<TGridItem>? FilterCriteria { get; set; }
    [Parameter] public RenderFragment? SelectedItemsActionDropDown { get; set; }
    [Parameter] public RenderFragment? FilterSection { get; set; }

    [Inject] protected IJSRuntime JS { get; set; } = default!;
    [Inject] protected IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] protected ILogger<QuickGridWrapper<TGridItem>> Logger { get; set; } = default!;

    // Resolve icon provider lazily with a safe default so the component doesn't throw if it's not registered in DI
    private IQuickGridIconProvider? _iconProvider;
    protected IQuickGridIconProvider IconProvider =>
        _iconProvider ??= ServiceProvider.GetService<IQuickGridIconProvider>() ?? new DefaultQuickGridIconProvider();

    private const string ColumnTitleSetupErrorMessage = "Non-critical: Failed to setup column titles for {Id}. Application continues to run without this feature.";

    private bool _titlesLoaded;
    private bool _isTableIndex;
    private bool _isInMemorySearch;
    private bool _showFilterSection;

    private int _previousHashCode;
    private string? _searchQuery;
    private string? _lastSearchQuery;

    private QuickGrid<TGridItem>? _grid;
    private PaginationState? _pagination;
    public ColumnConfig? _selectedConfiguration;
    private ColumnManager<TGridItem> _defaultColumnManager = new();
    public ColumnManager<TGridItem> _usedColumnManager = new();

    private List<string> _defaultVisibleColumns = [];
    private List<TGridItem>? _evaluatedItems;
    public List<ColumnConfig> _columnConfigurations = [];
    private readonly List<FooterColumn<TGridItem>> _footerColumns = [];

    private int _selectedItemsCount => _filteredItems?.Count(item => (ISelectionDto?)item != null && ((ISelectionDto?)item)!.IsSelected) ?? 0;

    private IQueryable<TGridItem>? _filteredItems
    {
        get
        {
            IQueryable<TGridItem>? result;

            if (string.IsNullOrEmpty(_searchQuery))
            {
                result = Items;

            }
            else if (FilterCriteria is null)
            {
                result = Items?.Where(item => QuickSearchAction(item, _searchQuery));
            }
            else
            {
                result = _evaluatedItems?.AsQueryable();
            }

            if (_searchQuery != _lastSearchQuery)
            {
                _ = SearchResultChanged.InvokeAsync(result?.ToList() ?? []);

                _lastSearchQuery = _searchQuery;
            }

            return result;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (IsPaginator)
        {
            _pagination = new PaginationState
            {
                ItemsPerPage = ItemsPerPage
            };
        }

        _defaultColumnManager = ColumnManager;

        //var result = await ColumnConfigurationService.GetConfigurationsAsync(Id, _authenticatedUserId, default);

        //if (result.IsSuccess)
        //{
        //    _columnConfigurations = result.Value;
        //}

        SetDefaultColumns();

        _isInMemorySearch = FilterCriteria is null;

        await Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Id is null || _titlesLoaded) return;

        if (_usedColumnManager.Columns.Count > 0)
        {
            await RefreshColumnTitlesAsync();

            _titlesLoaded = true;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        SetTableIndex();

        UpdateSearchQuery();

        var currentHashCode = Items?.GetHashCode();

        if (_previousHashCode != currentHashCode)
        {
            _previousHashCode = currentHashCode ?? 0;

            await Task.Delay(10);

            await AddOrUpdateFooterAsync();
        }

        if (_defaultVisibleColumns.Count == 0)
        {
            SetDefaultColumns();
        }
    }

    private void UpdateSearchQuery()
    {
        if (string.IsNullOrWhiteSpace(QuickSearch) || _searchQuery == QuickSearch) return;

        _searchQuery = QuickSearch;
    }

    public async Task RefreshColumnTitlesAsync()
    {
        if (!_usedColumnManager.Columns.Any(w => w.Visible)) return;

        var titles = _usedColumnManager.Columns.Where(w => w.Visible).Select(col => col.FullTitle).ToList();

        try
        {
            await JS.InvokeVoidAsync("setColumnTitles", Id, titles);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ColumnTitleSetupErrorMessage, Id);
        }
    }

    public async ValueTask AddOrUpdateFooterAsync()
    {
        if (Id is null || _usedColumnManager.FooterColumns.Count == 0) return;

        await JS.InvokeVoidAsync("addOrUpdateFooter", Id, GenerateTableFooterWithTotals());
    }

    private void SetDefaultColumns()
    {
        InitializeDefaultColumnVisibility();

        _selectedConfiguration = _columnConfigurations.FirstOrDefault(w => w.Default);

        if (_selectedConfiguration != null)
        {
            SetColumnVisibility(_selectedConfiguration);
        }

        _usedColumnManager = _defaultColumnManager;

        //StateHasChanged();

        //foreach (var column in _defaultColumnManager.Columns)
        //{
        //    column.Visible = _selectedConfiguration.IsColumnSelected(column.FullTitle);

        //    _usedColumnManager.Add(column);
        //}

        //ColumnManager.SetVisibleColumns(_selectedConfiguration.ColumnSelections.Select(s => s.ColumnName));

        //ColumnManager.ResetColumnVisibility();
    }

    private void InitializeDefaultColumnVisibility()
        => _defaultVisibleColumns = _defaultColumnManager.Columns
            .Where(w => w.Visible && w.FullTitle != null)
            .Select(s => s.FullTitle!)
            .ToList();

    private string GenerateTableFooterWithTotals()
    {
        StringBuilder html = new();

        foreach (var column in _usedColumnManager.Columns.Where(w => w.Visible && w.Class != "d-none"))
        {
            var footerColumn = _usedColumnManager.FooterColumns.FirstOrDefault(w => w.Id == column.Id);

            if (footerColumn is null)
            {
                html.Append("<td></td>");
            }
            else if (footerColumn.Content != null)
            {
                html.Append(footerColumn.Content);
            }
            else if (Items != null)
            {
                html.Append(footerColumn.StringContent?.Invoke(Items));
            }
        }

        return $"<tfoot><tr class=\"table-warning fw-bold\">{html}</tr></tfoot>";
    }

    private async Task SearchTextChanged(string? text)
    {
        if (FilterCriteria is null || Items is null) return;

        if (string.IsNullOrWhiteSpace(text))
        {
            ClearSearch();

            return;
        }
        else if (text.Length < 3)
        {
            _searchQuery = text;

            return;
        }
        else
        {
            _searchQuery = text;
        }

        FilterCriteria.SearchTerm = _searchQuery;

        IsLoading = true;

        _evaluatedItems = await Items.Where(FilterCriteria.CreateExpression()).ToListAsync();

        IsLoading = false;
    }

    private void ClearSearch()
    {
        //_filteredItems = Items;
        _searchQuery = null;
    }

    public bool QuickSearchAction(TGridItem item, string query)
        => QuickSearchUtility.QuickSearch(item, query, IsNestedSearch);

    public Task ExportAsync() => Events?.OnExport.InvokeAsync(_filteredItems) ?? Task.CompletedTask;

    public async Task ExportSelectedColumnsAsync()
    {
        if (Events is null) return;

        if (_filteredItems is null)
        {
            await Events.WarningRequested.InvokeAsync("No items to export.");

            return;
        }

        var visibleColumns = _usedColumnManager.Columns
            .Where(w => w.Visible && w.PropertyName != null)
            .Select(s => s.PropertyName!)
            .Distinct()
            .ToList();

        if (visibleColumns.Count == 0)
        {
            await Events.WarningRequested.InvokeAsync("No columns to export. Contact Vaclav if this should be working.");

            return;
        }

        var expandoHelper = new ExpandoObjectBuilder<TGridItem>();

        var exportItems = _filteredItems.ToList()
            .Select(item => expandoHelper.Create(item, visibleColumns))
            .Where(obj => obj != null);

        await Events.OnSelectedColumnsExport.InvokeAsync(exportItems);
    }

    public async Task RefreshDataAsync()
    {
        if (_grid is null) return;

        await _grid.RefreshDataAsync();
    }

    public void SetTableIndex()
    {
        _isTableIndex = _usedColumnManager.Columns.Where(w => w.Visible).Any(x => x.Title == "#") && _usedColumnManager.IsIndexColumn;
    }

    private void UnselectAllItems()
    {
        if (Items is null) return;

        foreach (var item in Items)
        {
            if (item is ISelectionDto selectionDto)
            {
                selectionDto.IsSelected = false;
            }
        }
    }

    private async Task ManageColumns()
    {
        if (Events?.OnResetViewToDefault is null && IsColumnSelection)
        {
            IsColumnItemsSelection = true;
        }

        if (Events?.OnManageColumns is null) return;

        if (Id is null)
        {
            await Events.WarningRequested.InvokeAsync("Table ID is not set. Please set the ID parameter to enable this feature.");

            return;
        }

        await Events.OnManageColumns.Value.InvokeAsync();
    }

    private async Task OnColumnSelectionChangedAsync(ColumnConfig? config = null)
    {
        if (config != null)
        {
            _selectedConfiguration = config;
        }

        if (ColumnSelectionChanged.HasDelegate)
        {
            await ColumnSelectionChanged.InvokeAsync();
        }

        await RefreshColumnTitlesAsync();
    }

    private void OnColumnSelectorClose()
    {
        Console.WriteLine("Test");
    }

    private async Task SelectView(ColumnConfig config)
    {
        _selectedConfiguration = config;

        SetColumnVisibility(_selectedConfiguration);

        //ColumnManager.SetVisibleColumns(config.ColumnSelections.Select(s => s.ColumnName));

        //ColumnManager.ResetColumnVisibility();

        if (Events is not null)
        {
            await Events.OnSelectView.InvokeAsync(config);
        }

        await OnColumnSelectionChangedAsync();
    }

    private async void ResetViewToDefault()
    {
        _selectedConfiguration = null;

        foreach (var column in _defaultColumnManager.Columns)
        {
            if (column.FullTitle is null) continue;

            column.Visible = _defaultVisibleColumns.Contains(column.FullTitle);
        }

        if (Id != null && Events is not null)
        {
            await Events.OnResetViewToDefault.InvokeAsync();
        }

        await OnColumnSelectionChangedAsync();
    }

    private void SetColumnVisibility(ColumnConfig config)
    {
        foreach (var column in _defaultColumnManager.Columns)
        {
            column.Visible = config.IsColumnSelected(column.FullTitle);
        }
    }

    //public string GetTableClass() => $"table table-sm {IsTableIndex()} table-striped small table-blazor table-fit table-thead-sticky table-bg-transparent mb-0";

    public string GetTableClass() => $"{Class} {IsTableIndex()}";

    public string IsTableIndex() => _isTableIndex ? "table-index" : "";

    public void ToggleFilterSection()
    {
        _showFilterSection = !_showFilterSection;
    }

    public void Dispose()
    {

    }
}