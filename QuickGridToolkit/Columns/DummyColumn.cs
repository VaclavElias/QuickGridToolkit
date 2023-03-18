using QuickGridToolkit.Dtos;

namespace QuickGridToolkit.Columns;

public class DummyColumn : ColumnBase<UserDto>
{
    //https://stackoverflow.com/questions/75060043/blazor-quickgrid-how-to-create-a-column-without-populating-renderfragment
    /// <summary>
    /// Dummy Property otherwise it doesn't render
    /// </summary>
    [Parameter] public UserDto? Property { get; set; }

    protected override void CellContent(RenderTreeBuilder builder, UserDto item)
        => builder.AddContent(0, "Hello");
}