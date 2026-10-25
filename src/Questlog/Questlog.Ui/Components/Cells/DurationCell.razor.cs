using Microsoft.AspNetCore.Components;
using Questlog.Core.Time;

namespace Questlog.Ui.Components.Cells;

public partial class DurationCell : ComponentBase
{
	[Inject]
	private IDisplayTimeFormatter Formatter { get; set; } = null!;

	[Parameter]
	public TimeSpan Duration { get; set; }

	private IReadOnlyList<string> Parts => Formatter.DurationParts(Duration);
}
