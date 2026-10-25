using Questlog.Core.Display;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Services;

public class DisplaySyncService : IDisplaySyncService
{
	public event EventHandler? DisplayChanged;

	public event EventHandler? PageChanged;

	public ActiveDisplay ActiveDisplay { get; private set; } = ActiveDisplay.Program;

	public int DisplayIndex { get; private set; }

	public int MaxItems => DisplayCycle.ItemsPerPage;

	public DisplaySyncService(ITimerService timerService)
	{
		timerService.TickItem += OnItemTick;
		timerService.TickPage += OnPageTick;
	}

	private void OnItemTick(object? sender, EventArgs e)
	{
		DisplayIndex = (DisplayIndex + 1) % MaxItems;
		DisplayChanged?.Invoke(this, EventArgs.Empty);
	}

	private void OnPageTick(object? sender, EventArgs e)
	{
		ActiveDisplay = ActiveDisplay == ActiveDisplay.Program ? ActiveDisplay.Games : ActiveDisplay.Program;
		PageChanged?.Invoke(this, EventArgs.Empty);
	}
}
