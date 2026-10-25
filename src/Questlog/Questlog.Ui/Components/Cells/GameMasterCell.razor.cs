using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Components.Cells;

public partial class GameMasterCell : ComponentBase
{
	[Parameter, EditorRequired]
	public string Name { get; set; } = string.Empty;
}
