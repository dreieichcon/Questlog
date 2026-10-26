using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Ui.Pages.Viewers;

namespace Questlog.Ui.Tests.Pages.Viewers;

[TestClass]
public class GamesBySystemViewTests : ViewerPageContext
{
	private IRenderedComponent<GamesBySystemView> RenderView(string eventId, params SystemShare[] shares)
	{
		var gameService = Substitute.For<IGameService>();
		gameService.GetSystemSharesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<IReadOnlyList<SystemShare>>(shares));
		Services.AddSingleton(gameService);
		Services.AddMudServices();
		JSInterop.Mode = JSRuntimeMode.Loose;

		return Render<GamesBySystemView>(parameters => parameters.Add(view => view.EventId, eventId));
	}

	[TestMethod]
	public void Render_ShowsViewNameAsTitle()
	{
		var view = RenderView("c1", new SystemShare("DnD", 6));

		Assert.AreEqual("Spielrunden pro System", view.Find("h1").TextContent);
	}

	[TestMethod]
	public void Render_WithShares_ShowsNameCountAndPercentageInLegend()
	{
		var view = RenderView("c1", new SystemShare("DnD", 6), new SystemShare("Sonstige", 2));

		var entries = view.FindAll(".legend-entry");

		Assert.AreEqual(2, entries.Count);
		Assert.AreEqual("DnD", entries[0].QuerySelector(".legend-name")!.TextContent);
		StringAssert.Contains(entries[0].QuerySelector(".legend-value")!.TextContent, "6 (75");
		StringAssert.Contains(entries[1].QuerySelector(".legend-value")!.TextContent, "2 (25");
	}

	[TestMethod]
	public void Render_WithoutShares_ShowsEmptyText()
	{
		var view = RenderView("c1");

		StringAssert.Contains(view.Markup, "Es gibt keine Spielrunden für diese Veranstaltung");
	}
}
