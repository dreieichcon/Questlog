using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Components.Cells;

public partial class PlayersCell : ComponentBase
{
	[Parameter]
	public int Current { get; set; }

	[Parameter]
	public int Max { get; set; }

	private string SeatsClass => Current < Max ? "seats-free" : string.Empty;
}
