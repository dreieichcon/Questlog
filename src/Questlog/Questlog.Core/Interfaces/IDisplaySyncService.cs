using Questlog.Core.Models;

namespace Questlog.Core.Interfaces;

public interface IDisplaySyncService
{
	public event EventHandler? DisplayChanged;

	public event EventHandler? PageChanged;

	public ActiveDisplay ActiveDisplay { get; }

	public int DisplayIndex { get; }

	public int MaxItems { get; }
}
