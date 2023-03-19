# QuickGridToolkit

This toolkit should help you to use the QuickGrid with more functionality, specifically, if the QuickGrid is used in multiple places, with similar data structure, but with different columns.

- **QuickGridToolkit** is a library with additional functionality for the [QuickGrid](https://aspnet.github.io/quickgridsamples/) with this functionality
  - [x] Add columns to the grid dynamically
  - [x] Add columns selection
  - [x] Predefined/Strong typed columns e.g. `AddCountry()`
  - [ ] Add columns sorting
  - [ ] Add custom columns (ImageColumn, TickColumn, ..)
  - [ ] Add clickable columns with call back
- **BlazorApp1** is a sample application that uses the QuickGridToolkit library, page Users

Example:

```razor
<h1>Users</h1>
<div class="my-3">
    <ColumnSelector ColumnManager="_columnManager" SelectionChanged="SelectionChangedAsync" />
</div>
<QuickGrid @ref="_grid" Items="@_items.AsQueryable()" Class="table table-sm table-index table-striped small table-blazor table-fit table-thead-sticky mb-0" Theme="twentyAI">
    @QuickGridColumns.Columns(_columnManager)
</QuickGrid>

@code {
    private List<UserDto> _items = new();
    private ColumnManager<UserDto> _columnManager = new();
    private QuickGrid<UserDto>? _grid;

    protected override void OnInitialized()
    {
        _columnManager.AddIndexColumn();
        _columnManager.Add(new() { Property = p => p.Id });
        _columnManager.Add(new() { Property = p => p.Name });
        _columnManager.Add(new() { Property = p => p.Age });
        _columnManager.AddCountry();

        _items = UserService.GetUsers();
    }

    private async Task SelectionChangedAsync()
    {
        if (_grid is null) return;

        await _grid.RefreshDataAsync();
    }
}

```

## Current Issues

- [ ] The `Format` property is not working for object type
