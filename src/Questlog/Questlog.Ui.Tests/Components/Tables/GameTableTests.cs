using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Ui.Components.Tables;

namespace Questlog.Ui.Tests.Components.Tables;

[TestClass]
public class GameTableTests : TableContext
{
	private readonly IGameService _gameService = Substitute.For<IGameService>();

	private static Game CreateGame(string title, DateTimeOffset start)
		=> new() { Id = title, Title = title, GameMaster = "Max", Start = start };

	private void SetupGames(params Game[] games)
		=> _gameService.GetAllAsync("con1", Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<IReadOnlyList<Game>>(games));

	private IRenderedComponent<GameTable> RenderTable()
	{
		Services.AddSingleton(_gameService);
		return Render<GameTable>(parameters => parameters.Add(table => table.EventId, "con1"));
	}

	private static string RowText(IRenderedComponent<GameTable> table, int index)
		=> table.FindAll("tbody tr")[index].TextContent;

	[TestMethod]
	public void Render_ShowsGamesOfTheEvent()
	{
		SetupGames(CreateGame("Drachenfels", Now.AddHours(1)));

		var table = RenderTable();

		Assert.AreEqual(1, table.FindAll("tbody tr").Count);
		StringAssert.Contains(RowText(table, 0), "Drachenfels");
	}

	[TestMethod]
	public void Render_GamesAlreadyStarted_AreHidden()
	{
		SetupGames(CreateGame("Alt", Now.AddMinutes(-1)), CreateGame("Neu", Now.AddMinutes(1)));

		var table = RenderTable();

		Assert.AreEqual(1, table.FindAll("tbody tr").Count);
		StringAssert.Contains(RowText(table, 0), "Neu");
	}

	[TestMethod]
	public void Render_GamesInArbitraryOrder_AreSortedByStart()
	{
		SetupGames(CreateGame("Spaet", Now.AddHours(3)), CreateGame("Frueh", Now.AddHours(1)));

		var table = RenderTable();

		StringAssert.Contains(RowText(table, 0), "Frueh");
		StringAssert.Contains(RowText(table, 1), "Spaet");
	}

	[TestMethod]
	public void Render_AllGamesOnOneDay_ShowsTimeWithoutWeekday()
	{
		SetupGames(CreateGame("Eins", Now.AddHours(1)), CreateGame("Zwei", Now.AddHours(2)));

		var table = RenderTable();

		StringAssert.Contains(RowText(table, 0), "08:00");
		Assert.IsFalse(RowText(table, 0).Contains("Mo."));
	}

	[TestMethod]
	public void Render_GamesOnDifferentDays_ShowsWeekdayWithTime()
	{
		SetupGames(CreateGame("Eins", Now.AddHours(1)), CreateGame("Zwei", Now.AddDays(1)));

		var table = RenderTable();

		StringAssert.Contains(RowText(table, 0), "Mo. 08:00");
		StringAssert.Contains(RowText(table, 1), "Di. 07:00");
	}

	[TestMethod]
	public void Render_NoUpcomingGames_ShowsEmptyText()
	{
		SetupGames(CreateGame("Alt", Now.AddHours(-1)));

		var table = RenderTable();

		Assert.AreEqual(0, table.FindAll("tbody tr").Count);
		StringAssert.Contains(table.Find(".empty-text").TextContent, "Spielrunden");
	}

	[TestMethod]
	public void TickMinute_ReloadsGames()
	{
		SetupGames(CreateGame("Eins", Now.AddHours(1)));
		var table = RenderTable();
		SetupGames(CreateGame("Eins", Now.AddHours(1)), CreateGame("Zwei", Now.AddHours(2)));

		RaiseMinuteTick();

		table.WaitForAssertion(() => Assert.AreEqual(2, table.FindAll("tbody tr").Count));
	}

	[TestMethod]
	public void TickMinute_GameHasStartedMeanwhile_RemovesGame()
	{
		SetupGames(CreateGame("Eins", Now.AddMinutes(30)));
		var table = RenderTable();
		TimeProvider.Advance(TimeSpan.FromMinutes(31));

		RaiseMinuteTick();

		table.WaitForAssertion(() => Assert.AreEqual(0, table.FindAll("tbody tr").Count));
	}

	[TestMethod]
	public async Task Dispose_RemovesMinuteTickSubscription()
	{
		SetupGames(CreateGame("Eins", Now.AddHours(1)));
		RenderTable();
		await DisposeComponentsAsync();
		_gameService.ClearReceivedCalls();

		RaiseMinuteTick();

		await _gameService.DidNotReceive().GetAllAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
	}
}
