using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Time;
using Questlog.Ui.Components;

namespace Questlog.Ui.Tests.Components;

[TestClass]
public class ScreenFrameTests : BunitContext
{
	private readonly IThemeService _themeService = Substitute.For<IThemeService>();

	private readonly ITimerService _timerService = Substitute.For<ITimerService>();

	private IRenderedComponent<ScreenFrame> RenderFrame(bool isDark = false)
	{
		var timeProvider = new FakeTimeProvider(new DateTimeOffset(2030, 7, 1, 10, 0, 0, TimeSpan.Zero));
		_themeService.IsDarkAsync(Arg.Any<CancellationToken>()).Returns(isDark);

		Services.AddSingleton<TimeProvider>(timeProvider);
		Services.AddSingleton<IDisplayTimeFormatter>(new DisplayTimeFormatter(timeProvider, TimeZoneInfo.Utc));
		Services.AddSingleton(_themeService);
		Services.AddSingleton(_timerService);

		return Render<ScreenFrame>(parameters => parameters
			.Add(frame => frame.Title, "Titel")
			.Add(frame => frame.FooterLeft, "links")
			.Add(frame => frame.FooterRight, "rechts"));
	}

	private static bool IsDarkTheme(IRenderedComponent<ScreenFrame> frame)
		=> frame.Find(".screen-frame").ClassList.Contains("theme-dark");

	[TestMethod]
	public void Render_ShowsTitleAndFooterContent()
	{
		var frame = RenderFrame();

		StringAssert.Contains(frame.Find("header").TextContent, "Titel");
		StringAssert.Contains(frame.Find("footer").TextContent, "links");
		StringAssert.Contains(frame.Find("footer").TextContent, "rechts");
	}

	[TestMethod]
	public void Render_ThemeIsDark_AddsDarkThemeClass()
	{
		var frame = RenderFrame(isDark: true);

		Assert.IsTrue(IsDarkTheme(frame));
	}

	[TestMethod]
	public void Render_ThemeIsLight_OmitsDarkThemeClass()
	{
		var frame = RenderFrame(isDark: false);

		Assert.IsFalse(IsDarkTheme(frame));
	}

	[TestMethod]
	public void TickMinute_ThemeChangesToDark_AddsDarkThemeClass()
	{
		var frame = RenderFrame(isDark: false);
		_themeService.IsDarkAsync(Arg.Any<CancellationToken>()).Returns(true);

		_timerService.TickMinute += Raise.Event();

		frame.WaitForAssertion(() => Assert.IsTrue(IsDarkTheme(frame)));
	}

	[TestMethod]
	public async Task Dispose_RemovesMinuteTickSubscription()
	{
		RenderFrame();
		await DisposeComponentsAsync();
		_themeService.ClearReceivedCalls();

		_timerService.TickMinute += Raise.Event();

		await _themeService.DidNotReceive().IsDarkAsync(Arg.Any<CancellationToken>());
	}
}
