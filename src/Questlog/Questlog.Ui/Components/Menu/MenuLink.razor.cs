using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Components.Menu;

public partial class MenuLink : ComponentBase
{
	[Parameter, EditorRequired]
	public string Href { get; set; } = string.Empty;

	[Parameter, EditorRequired]
	public string Text { get; set; } = string.Empty;
}
