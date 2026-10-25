using Microsoft.AspNetCore.Components;
using Questlog.Core.Time;

namespace Questlog.Ui.Components.Cells;

public partial class StartTimeCell : ComponentBase
{
	[Inject]
	private IDisplayTimeFormatter Formatter { get; set; } = null!;

	[Parameter]
	public DateTimeOffset Start { get; set; }

	[Parameter]
	public bool ShowDay { get; set; }

	private string StartText => ShowDay ? Formatter.DayAndTime(Start) : Formatter.Time(Start);
}
