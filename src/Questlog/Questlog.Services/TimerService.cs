using Microsoft.Extensions.Logging;
using Questlog.Core.Display;
using Questlog.Core.Interfaces;

namespace Questlog.Services;

public class TimerService : ITimerService, IDisposable
{
	private static readonly TimeSpan MinuteInterval = TimeSpan.FromMinutes(1);

	private readonly ILogger<TimerService>? _logger;

	private readonly ITimer _itemTimer;

	private readonly ITimer _minuteTimer;

	private int _elapsedItems;

	public event EventHandler? TickItem;

	public event EventHandler? TickPage;

	public event EventHandler? TickMinute;

	public TimerService(TimeProvider timeProvider, ILogger<TimerService>? logger = null)
	{
		_logger = logger;
		_itemTimer = timeProvider.CreateTimer(_ => OnItemTick(), null, DisplayCycle.ItemInterval, DisplayCycle.ItemInterval);
		_minuteTimer = timeProvider.CreateTimer(_ => Raise(TickMinute), null, MinuteInterval, MinuteInterval);
	}

	/// <summary>
	/// Stops all timers.
	/// </summary>
	public void Dispose()
	{
		_itemTimer.Dispose();
		_minuteTimer.Dispose();
		GC.SuppressFinalize(this);
	}

	private void OnItemTick()
	{
		Raise(TickItem);

		_elapsedItems++;

		if (_elapsedItems % DisplayCycle.ItemsPerPage == 0)
			Raise(TickPage);
	}

	private void Raise(EventHandler? handler)
	{
		if (handler is null)
			return;

		foreach (var subscriber in handler.GetInvocationList().Cast<EventHandler>())
		{
			try
			{
				subscriber(this, EventArgs.Empty);
			}
			catch (Exception exception)
			{
				_logger?.LogError(exception, "A timer subscriber failed.");
			}
		}
	}
}
