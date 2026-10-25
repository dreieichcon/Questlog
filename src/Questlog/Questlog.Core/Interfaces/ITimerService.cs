namespace Questlog.Core.Interfaces;

public interface ITimerService
{
	public event EventHandler? TickItem;

	public event EventHandler? TickPage;

	public event EventHandler? TickMinute;
}
