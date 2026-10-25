using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Components;

public partial class ScreenFrame : ComponentBase
{
	[Parameter, EditorRequired]
	public string Title { get; set; } = string.Empty;

	[Parameter]
	public string? LogoUrl { get; set; }

	[Parameter]
	public RenderFragment? FooterLeft { get; set; }

	[Parameter]
	public RenderFragment? FooterRight { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }
}
