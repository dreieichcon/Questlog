using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Ui.Pages.Viewers;

namespace Questlog.Ui.Tests.Pages.Viewers;

[TestClass]
public class ProgramAndGamesViewTests : ViewerPageContext
{
	private IRenderedComponent<ProgramAndGamesView> RenderView(int programCount, int gameCount)
	{
		var items = Enumerable.Range(1, programCount)
			.Select(index => new ProgramItem { Id = $"{index}", Title = $"Punkt {index}", Start = Now.AddHours(index) })
			.ToList();
		var games = Enumerable.Range(1, gameCount)
			.Select(index => new Game { Id = $"{index}", Title = $"Runde {index}", GameMaster = "Max", Start = Now.AddHours(index) })
			.ToList();
		var programService = Substitute.For<IProgramService>();
		programService.GetAllAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<IReadOnlyList<ProgramItem>>(items));
		var gameService = Substitute.For<IGameService>();
		gameService.GetAllAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<IReadOnlyList<Game>>(games));
		Services.AddSingleton(programService);
		Services.AddSingleton(gameService);

		return Render<ProgramAndGamesView>(parameters => parameters.Add(view => view.EventId, "c1"));
	}

	private static bool IsHidden(IRenderedComponent<ProgramAndGamesView> view, int paneIndex)
		=> view.FindAll(".data-table")[paneIndex].ClassList.Contains("table-hidden");

	private static string Footer(IRenderedComponent<ProgramAndGamesView> view)
		=> view.Find("footer").TextContent;

	[TestMethod]
	public void Render_ProgramActive_ShowsProgramPaneAndProgramCycle()
	{
		DisplaySync.ActiveDisplay.Returns(ActiveDisplay.Program);

		var view = RenderView(programCount: 3, gameCount: 5);

		view.WaitForAssertion(() => StringAssert.Contains(Footer(view), "1 / 3"));
		StringAssert.Contains(Footer(view), "Programm");
		Assert.IsFalse(IsHidden(view, 0));
		Assert.IsTrue(IsHidden(view, 1));
	}

	[TestMethod]
	public void Render_GamesActive_ShowsGamesPaneAndGamesCycle()
	{
		DisplaySync.ActiveDisplay.Returns(ActiveDisplay.Games);

		var view = RenderView(programCount: 3, gameCount: 5);

		view.WaitForAssertion(() => StringAssert.Contains(Footer(view), "1 / 5"));
		StringAssert.Contains(Footer(view), "Spielrunden");
		Assert.IsTrue(IsHidden(view, 0));
		Assert.IsFalse(IsHidden(view, 1));
	}

	[TestMethod]
	public void Render_BothTablesAreLoaded()
	{
		var view = RenderView(programCount: 3, gameCount: 5);

		Assert.AreEqual(8, view.FindAll("tbody tr").Count);
	}

	[TestMethod]
	public void Render_ActiveTableIsEmpty_SkipsToOtherTable()
	{
		DisplaySync.ActiveDisplay.Returns(ActiveDisplay.Program);

		var view = RenderView(programCount: 0, gameCount: 4);

		view.WaitForAssertion(() => StringAssert.Contains(Footer(view), "Spielrunden"));
		Assert.IsTrue(IsHidden(view, 0));
		Assert.IsFalse(IsHidden(view, 1));
	}

	[TestMethod]
	public void Render_BothTablesEmpty_ShowsActiveTable()
	{
		DisplaySync.ActiveDisplay.Returns(ActiveDisplay.Games);

		var view = RenderView(programCount: 0, gameCount: 0);

		StringAssert.Contains(Footer(view), "Spielrunden");
		Assert.IsFalse(IsHidden(view, 1));
	}

	[TestMethod]
	public void PageChanged_SwitchesShownTableAndFooter()
	{
		DisplaySync.ActiveDisplay.Returns(ActiveDisplay.Program);
		var view = RenderView(programCount: 3, gameCount: 5);
		DisplaySync.ActiveDisplay.Returns(ActiveDisplay.Games);

		DisplaySync.PageChanged += Raise.Event();

		view.WaitForAssertion(() => StringAssert.Contains(Footer(view), "Spielrunden"));
		Assert.IsTrue(IsHidden(view, 0));
		Assert.IsFalse(IsHidden(view, 1));
	}
}
