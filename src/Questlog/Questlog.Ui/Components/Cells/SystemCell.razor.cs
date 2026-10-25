using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Components.Cells;

public partial class SystemCell : ComponentBase
{
	[Parameter, EditorRequired]
	public string Name { get; set; } = string.Empty;

	[Parameter]
	public string? Version { get; set; }

	private bool ShowVersion => !string.IsNullOrEmpty(Version) && !Name.Contains(Version);
}
