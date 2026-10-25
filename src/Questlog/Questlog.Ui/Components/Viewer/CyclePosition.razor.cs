using Microsoft.AspNetCore.Components;
using Questlog.Core.Interfaces;

namespace Questlog.Ui.Components.Viewer;

public partial class CyclePosition : ComponentBase, IDisposable
{
	[Inject]
	private IDisplaySyncService DisplaySyncService { get; set; } = null!;

	[Parameter]
	public int Length { get; set; }

	private int Position => DisplaySyncService.DisplayIndex % Length + 1;

	protected override void OnInitialized()
		=> DisplaySyncService.DisplayChanged += OnDisplayChanged;

	/// <summary>
	/// Removes the subscription to the display changes.
	/// </summary>
	public void Dispose()
		=> DisplaySyncService.DisplayChanged -= OnDisplayChanged;

	private void OnDisplayChanged(object? sender, EventArgs e)
		=> _ = InvokeAsync(StateHasChanged);
}
