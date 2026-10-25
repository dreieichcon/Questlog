using Microsoft.AspNetCore.Components;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Ui.Components.Viewer;

public partial class ViewerFrame : ComponentBase
{
	private int _cycleLength;

	private string? _loadedEventId;

	[Inject]
	private IConventionService ConventionService { get; set; } = null!;

	[Parameter, EditorRequired]
	public string EventId { get; set; } = string.Empty;

	[Parameter]
	public string ViewName { get; set; } = string.Empty;

	[Parameter]
	public int? CycleLength { get; set; }

	[Parameter]
	public RenderFragment<ViewerFrame>? ChildContent { get; set; }

	private Convention? Convention { get; set; }

	private bool IsLoaded { get; set; }

	protected override async Task OnParametersSetAsync()
	{
		if (EventId == _loadedEventId)
			return;

		_loadedEventId = EventId;
		Convention = await ConventionService.GetAsync(EventId);
		IsLoaded = true;
	}

	/// <summary>
	/// Sets the number of rows the display cycles through.
	/// </summary>
	public void SetCycleLength(int length)
		=> _cycleLength = length;
}
