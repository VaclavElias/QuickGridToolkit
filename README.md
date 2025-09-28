# QuickGrid.Toolkit

This toolkit should help you to use the QuickGrid with more functionality, specifically, if the QuickGrid is used in multiple places, with similar data structure, but with different columns.

- **QuickGrid.Toolkit** is a library with additional functionality for the [QuickGrid](https://aspnet.github.io/quickgridsamples/) with this functionality
  - ✅ Add columns to the grid dynamically
  - ✅ Add columns selection
  - ✅ Predefined/Strong typed columns e.g. `AddCountry()`
  - ✅ Add columns sorting
  - ✅ table-index
  - ✅ table-fit
  - ✅ table-thead-sticky
  - ⏳ Add custom ImageColumn
  - ⏳ Add custom TickColumn
  - ⏳ Add clickable columns with call back
- **QuickGrid.Samples** is a sample application that uses the QuickGridToolkit library, page Users

## Requirements

- .NET 10
- Bootstrap 5
- `<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.13.1/font/bootstrap-icons.min.css">` or alternative icons. You can use `IQuickGridIconProvider` for custom icons.
- `<link rel="stylesheet" href="@Assets["_content/QuickGrid.Toolkit/app.css"]" />`

## Regular QuickGrid Example:

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

## QuickGridWrapper Example:

You would use this one if you have multiple grids with similar data structure, but different columns.

```razor
WIP
```


## Current Issues

- [ ] The `Format` property is not working for object type

