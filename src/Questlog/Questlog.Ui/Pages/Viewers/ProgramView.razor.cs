using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Pages.Viewers;

public partial class ProgramView : ComponentBase
{
	[Parameter]
	public string EventId { get; set; } = string.Empty;
}
