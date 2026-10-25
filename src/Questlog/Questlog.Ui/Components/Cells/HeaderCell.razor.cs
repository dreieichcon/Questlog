using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Components.Cells;

public partial class HeaderCell : ComponentBase
{
	[Parameter]
	public int? Width { get; set; }

	[Parameter]
	public bool Small { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private string? WidthStyle => Width.HasValue ? $"width: {Width}%;" : null;
}
