using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Components.Menu;

public partial class MenuGrid : ComponentBase
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }
}
