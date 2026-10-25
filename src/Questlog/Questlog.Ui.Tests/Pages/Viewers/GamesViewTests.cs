using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Ui.Pages.Viewers;

namespace Questlog.Ui.Tests.Pages.Viewers;

[TestClass]
public class GamesViewTests : ViewerPageContext
{
	private IRenderedComponent<GamesView> RenderView(string eventId, int gameCount)
	{
		var games = Enumerable.Range(1, gameCount)
			.Select(index => new Game { Id = $"{index}", Title = $"Runde {index}", GameMaster = "Max", Start = Now.AddHours(index) })
			.ToList();
		var gameService = Substitute.For<IGameService>();
		gameService.GetAllAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<IReadOnlyList<Game>>(games));
		Services.AddSingleton(gameService);

		return Render<GamesView>(parameters => parameters.Add(view => view.EventId, eventId));
	}

	[TestMethod]
	public void Render_ShowsConventionAndViewName()
	{
		var view = RenderView("c1", gameCount: 2);

		StringAssert.Contains(view.Find("header").TextContent, "Alpha Con");
		StringAssert.Contains(view.Find("footer").TextContent, "Spielrunden");
	}

	[TestMethod]
	public void Render_ShowsGameRowsAndCycle()
	{
		var view = RenderView("c1", gameCount: 4);

		Assert.AreEqual(4, view.FindAll("tbody tr").Count);
		view.WaitForAssertion(() => StringAssert.Contains(view.Find("footer").TextContent, "1 / 4"));
	}

	[TestMethod]
	public void Render_UnknownEvent_ShowsNotFoundTextWithoutFrame()
	{
		var view = RenderView("missing", gameCount: 2);

		StringAssert.Contains(view.Markup, "Die Veranstaltung missing existiert nicht");
		Assert.AreEqual(0, view.FindAll("header").Count);
	}
}
