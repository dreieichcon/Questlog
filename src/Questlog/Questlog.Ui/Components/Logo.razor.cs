using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Components;

public partial class Logo : ComponentBase
{
	[Parameter]
	public string? Url { get; set; }

	[Parameter]
	public string Size { get; set; } = "6rem";
}
