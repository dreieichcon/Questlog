using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Components.Menu;

public partial class MenuContainer : ComponentBase
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }
}
