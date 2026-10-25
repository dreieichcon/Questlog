using Microsoft.AspNetCore.Components;
using Questlog.Core.Display;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Ui.Components.Tables;

public abstract class EventTableBase<TItem> : ComponentBase, IDisposable where TItem : IHasStartTime
{
	private string? _loadedEventId;

	[Inject]
	private ITimerService TimerService { get; set; } = null!;

	[Inject]
	private TimeProvider Time { get; set; } = null!;

	[Inject]
	private TimeZoneInfo TimeZone { get; set; } = null!;

	[Parameter, EditorRequired]
	public string EventId { get; set; } = string.Empty;

	[Parameter]
	public EventCallback<int> CycleLengthChanged { get; set; }

	[Parameter]
	public bool IsShown { get; set; } = true;

	protected IReadOnlyList<TItem> Items { get; private set; } = [];

	protected bool ShowDay { get; private set; }

	protected abstract Task<IReadOnlyList<TItem>> LoadAsync(string eventId);

	protected override void OnInitialized()
		=> TimerService.TickMinute += OnTickMinute;

	protected override async Task OnParametersSetAsync()
	{
		if (EventId == _loadedEventId)
			return;

		_loadedEventId = EventId;
		await ReloadAsync();
	}

	/// <summary>
	/// Removes the subscription to the minute tick.
	/// </summary>
	public void Dispose()
		=> TimerService.TickMinute -= OnTickMinute;

	private void OnTickMinute(object? sender, EventArgs e)
		=> _ = InvokeAsync(ReloadAndRenderAsync);

	private async Task ReloadAndRenderAsync()
	{
		await ReloadAsync();
		StateHasChanged();
	}

	private async Task ReloadAsync()
	{
		var now = Time.GetUtcNow();
		var items = await LoadAsync(EventId);

		Items = [.. items.Where(item => item.Start > now).OrderBy(item => item.Start)];
		ShowDay = SpansMultipleDays(Items);
		await CycleLengthChanged.InvokeAsync(Math.Min(Items.Count, DisplayCycle.ItemsPerPage));
	}

	private bool SpansMultipleDays(IReadOnlyList<TItem> items)
		=> items.Select(item => TimeZoneInfo.ConvertTime(item.Start, TimeZone).Date).Distinct().Count() > 1;
}
