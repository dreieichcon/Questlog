using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Time;

namespace Questlog.Ui.Tests.Components.Tables;

public abstract class TableContext : BunitContext
{
	protected static readonly DateTimeOffset Now = new(2030, 7, 1, 7, 0, 0, TimeSpan.Zero);

	protected readonly FakeTimeProvider TimeProvider = new(Now);

	protected readonly ITimerService TimerService = Substitute.For<ITimerService>();

	protected TableContext()
	{
		var displaySync = Substitute.For<IDisplaySyncService>();
		displaySync.MaxItems.Returns(20);

		Services.AddSingleton<TimeProvider>(TimeProvider);
		Services.AddSingleton(TimeZoneInfo.Utc);
		Services.AddSingleton<IDisplayTimeFormatter>(new DisplayTimeFormatter(TimeProvider, TimeZoneInfo.Utc));
		Services.AddSingleton(TimerService);
		Services.AddSingleton(displaySync);
		JSInterop.Mode = JSRuntimeMode.Loose;
	}

	protected void RaiseMinuteTick()
		=> TimerService.TickMinute += Raise.Event();
}
