using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Questlog.Core.Interfaces;

namespace Questlog.Ui.Components;

public partial class DataTable<TItem> : ComponentBase, IDisposable
{
	[Inject]
	private IDisplaySyncService DisplaySyncService { get; set; } = null!;

	[Inject]
	private IJSRuntime JsRuntime { get; set; } = null!;

	[Parameter]
	public IReadOnlyList<TItem> Items { get; set; } = [];

	[Parameter]
	public RenderFragment? Header { get; set; }

	[Parameter, EditorRequired]
	public RenderFragment<TItem> RowTemplate { get; set; } = null!;

	[Parameter]
	public string EmptyText { get; set; } = string.Empty;

	private int ActiveIndex => Items.Count == 0 ? -1 : DisplaySyncService.DisplayIndex % Math.Min(Items.Count, DisplaySyncService.MaxItems);

	protected override void OnInitialized()
		=> DisplaySyncService.DisplayChanged += OnDisplayChanged;

	/// <summary>
	/// Removes the subscription to the display changes.
	/// </summary>
	public void Dispose()
		=> DisplaySyncService.DisplayChanged -= OnDisplayChanged;

	private static string RowId(int index) => $"row-{index}";

	private void OnDisplayChanged(object? sender, EventArgs e)
		=> _ = InvokeAsync(ShowActiveRowAsync);

	private async Task ShowActiveRowAsync()
	{
		StateHasChanged();

		if (ActiveIndex < 0)
			return;

		try
		{
			await JsRuntime.InvokeVoidAsync("scrollToElement", RowId(ActiveIndex));
		}
		catch (JSDisconnectedException)
		{
			// the circuit is gone, nothing to scroll
		}
	}
}
