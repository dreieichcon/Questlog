using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Questlog.Core.Time;
using Questlog.Ui.Components;

namespace Questlog.Ui.Tests.Components;

[TestClass]
public class ScreenFrameTests : BunitContext
{
	private IRenderedComponent<ScreenFrame> RenderFrame(string? logoUrl = null)
	{
		var timeProvider = new FakeTimeProvider(new DateTimeOffset(2030, 7, 1, 10, 0, 0, TimeSpan.Zero));
		Services.AddSingleton<TimeProvider>(timeProvider);
		Services.AddSingleton<IDisplayTimeFormatter>(new DisplayTimeFormatter(timeProvider, TimeZoneInfo.Utc));

		return Render<ScreenFrame>(parameters => parameters
			.Add(frame => frame.Title, "Titel")
			.Add(frame => frame.LogoUrl, logoUrl)
			.Add(frame => frame.FooterLeft, "links")
			.Add(frame => frame.FooterRight, "rechts")
			.AddChildContent("<p>Inhalt</p>"));
	}

	[TestMethod]
	public void Render_ShowsTitleAndFooterContent()
	{
		var frame = RenderFrame();

		StringAssert.Contains(frame.Find("header").TextContent, "Titel");
		StringAssert.Contains(frame.Find("footer").TextContent, "links");
		StringAssert.Contains(frame.Find("footer").TextContent, "rechts");
	}

	[TestMethod]
	public void Render_ShowsContent()
	{
		Assert.AreEqual("Inhalt", RenderFrame().Find("main p").TextContent);
	}

	[TestMethod]
	public void Render_WithLogoUrl_ShowsLogo()
	{
		var frame = RenderFrame("https://example.org/logo.png");

		Assert.AreEqual("https://example.org/logo.png", frame.Find("header img").GetAttribute("src"));
	}

	[TestMethod]
	public void Render_WithoutLogoUrl_ShowsNoLogo()
	{
		Assert.AreEqual(0, RenderFrame().FindAll("header img").Count);
	}
}
