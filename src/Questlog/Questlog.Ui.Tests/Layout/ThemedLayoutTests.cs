using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Ui.Layout;

namespace Questlog.Ui.Tests.Layout;

[TestClass]
public class ThemedLayoutTests : BunitContext
{
	private readonly IThemeService _themeService = Substitute.For<IThemeService>();

	private readonly ITimerService _timerService = Substitute.For<ITimerService>();

	private IRenderedComponent<ThemedLayout> RenderLayout(bool isDark = false)
	{
		_themeService.IsDarkAsync(Arg.Any<CancellationToken>()).Returns(isDark);
		Services.AddSingleton(_themeService);
		Services.AddSingleton(_timerService);

		return Render<ThemedLayout>(parameters => parameters.Add(layout => layout.Body, "<p>Inhalt</p>"));
	}

	private static bool IsDarkTheme(IRenderedComponent<ThemedLayout> layout)
		=> layout.Find(".theme-root").ClassList.Contains("theme-dark");

	[TestMethod]
	public void Render_ShowsBody()
	{
		var layout = RenderLayout();

		Assert.AreEqual("Inhalt", layout.Find("p").TextContent);
	}

	[TestMethod]
	public void Render_ThemeIsDark_AddsDarkThemeClass()
	{
		Assert.IsTrue(IsDarkTheme(RenderLayout(isDark: true)));
	}

	[TestMethod]
	public void Render_ThemeIsLight_OmitsDarkThemeClass()
	{
		Assert.IsFalse(IsDarkTheme(RenderLayout(isDark: false)));
	}

	[TestMethod]
	public void TickMinute_ThemeChangesToDark_AddsDarkThemeClass()
	{
		var layout = RenderLayout(isDark: false);
		_themeService.IsDarkAsync(Arg.Any<CancellationToken>()).Returns(true);

		_timerService.TickMinute += Raise.Event();

		layout.WaitForAssertion(() => Assert.IsTrue(IsDarkTheme(layout)));
	}

	[TestMethod]
	public async Task Dispose_RemovesMinuteTickSubscription()
	{
		RenderLayout();
		await DisposeComponentsAsync();
		_themeService.ClearReceivedCalls();

		_timerService.TickMinute += Raise.Event();

		await _themeService.DidNotReceive().IsDarkAsync(Arg.Any<CancellationToken>());
	}
}
