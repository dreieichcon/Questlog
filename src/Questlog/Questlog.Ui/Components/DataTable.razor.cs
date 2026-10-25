using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Questlog.Core.Interfaces;

namespace Questlog.Ui.Components;

public partial class DataTable<TItem> : ComponentBase, IDisposable
{
	private ElementReference _container;

	private bool _wasShown = true;

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

	[Parameter]
	public string RowIdPrefix { get; set; } = "row";

	[Parameter]
	public bool IsShown { get; set; } = true;

	private int ActiveIndex => Items.Count == 0 ? -1 : DisplaySyncService.DisplayIndex % Math.Min(Items.Count, DisplaySyncService.MaxItems);

	protected override void OnInitialized()
		=> DisplaySyncService.DisplayChanged += OnDisplayChanged;

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (_wasShown && !IsShown)
			await InvokeJsAsync("resetScroll", _container);

		_wasShown = IsShown;
	}

	/// <summary>
	/// Removes the subscription to the display changes.
	/// </summary>
	public void Dispose()
		=> DisplaySyncService.DisplayChanged -= OnDisplayChanged;

	private string RowId(int index) => $"{RowIdPrefix}-{index}";

	private void OnDisplayChanged(object? sender, EventArgs e)
		=> _ = InvokeAsync(ShowActiveRowAsync);

	private async Task ShowActiveRowAsync()
	{
		StateHasChanged();

		if (IsShown && ActiveIndex >= 0)
			await InvokeJsAsync("scrollToElement", RowId(ActiveIndex));
	}

	private async Task InvokeJsAsync(string function, params object[] arguments)
	{
		try
		{
			await JsRuntime.InvokeVoidAsync(function, arguments);
		}
		catch (JSDisconnectedException)
		{
			// the circuit is gone, nothing to scroll
		}
	}
}
