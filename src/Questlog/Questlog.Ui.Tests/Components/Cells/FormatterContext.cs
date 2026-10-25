using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Questlog.Core.Time;

namespace Questlog.Ui.Tests.Components.Cells;

public abstract class FormatterContext : BunitContext
{
	protected static readonly DateTimeOffset Now = new(2030, 7, 1, 7, 0, 0, TimeSpan.Zero);

	protected void AddFormatter()
	{
		var timeProvider = new FakeTimeProvider(Now);

		Services.AddSingleton<TimeProvider>(timeProvider);
		Services.AddSingleton<IDisplayTimeFormatter>(new DisplayTimeFormatter(timeProvider, TimeZoneInfo.Utc));
	}
}
