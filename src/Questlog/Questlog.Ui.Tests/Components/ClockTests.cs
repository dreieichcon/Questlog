using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Questlog.Core.Time;
using Questlog.Ui.Components;

namespace Questlog.Ui.Tests.Components;

[TestClass]
public class ClockTests : BunitContext
{
	private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2030, 7, 1, 10, 5, 9, TimeSpan.Zero));

	private IRenderedComponent<Clock> RenderClock()
	{
		var formatter = new DisplayTimeFormatter(_timeProvider, TimeZoneInfo.Utc);
		Services.AddSingleton<TimeProvider>(_timeProvider);
		Services.AddSingleton<IDisplayTimeFormatter>(formatter);

		return Render<Clock>();
	}

	[TestMethod]
	public void Render_ShowsCurrentTimeWithSeconds()
	{
		var clock = RenderClock();

		Assert.AreEqual("10:05:09", clock.Find(".clock").TextContent);
	}

	[TestMethod]
	public void Render_AfterOneSecond_ShowsUpdatedTime()
	{
		var clock = RenderClock();

		_timeProvider.Advance(TimeSpan.FromSeconds(1));

		clock.WaitForAssertion(() => Assert.AreEqual("10:05:10", clock.Find(".clock").TextContent));
	}

	[TestMethod]
	public void Dispose_StopsTheTimer()
	{
		var clock = RenderClock();
		var renderCount = clock.RenderCount;

		clock.Instance.Dispose();
		_timeProvider.Advance(TimeSpan.FromSeconds(5));

		Assert.AreEqual(renderCount, clock.RenderCount);
	}
}
