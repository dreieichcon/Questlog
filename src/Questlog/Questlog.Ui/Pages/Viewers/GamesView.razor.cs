using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Pages.Viewers;

public partial class GamesView : ComponentBase
{
	[Parameter]
	public string EventId { get; set; } = string.Empty;
}
