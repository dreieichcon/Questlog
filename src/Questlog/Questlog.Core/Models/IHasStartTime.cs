namespace Questlog.Core.Models;

public interface IHasStartTime
{
	public DateTimeOffset Start { get; }
}
