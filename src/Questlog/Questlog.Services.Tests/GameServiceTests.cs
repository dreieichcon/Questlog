using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Services.Tests.Helpers;
using Questlog.TestData;

namespace Questlog.Services.Tests;

[TestClass]
public class GameServiceTests
{
	private readonly IConservicesRepository _repository = Substitute.For<IConservicesRepository>();

	private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2030, 11, 14, 10, 0, 0, TimeSpan.Zero));

	private GameService CreateService() => new(_repository, _timeProvider);

	private void SetupGames(string eventId, params Game[] games)
		=> _repository.GetGamesAsync(eventId, Arg.Any<CancellationToken>()).Returns(ResultFactory.Success(games));

	[TestMethod]
	public async Task GetAllAsync_ReturnsGamesOfRequestedEvent()
	{
		SetupGames("event-1", ModelFactory.CreateGame("g1"), ModelFactory.CreateGame("g2"));
		var service = CreateService();

		var result = await service.GetAllAsync("event-1");

		CollectionAssert.AreEqual(new[] { "g1", "g2" }, result.Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public async Task GetAllAsync_WithinFiveMinutes_QueriesRepositoryOnce()
	{
		SetupGames("event-1", ModelFactory.CreateGame("g1"));
		var service = CreateService();

		await service.GetAllAsync("event-1");
		_timeProvider.Advance(TimeSpan.FromMinutes(4));
		await service.GetAllAsync("event-1");

		await _repository.Received(1).GetGamesAsync("event-1", Arg.Any<CancellationToken>());
	}

	[TestMethod]
	public async Task GetAllAsync_AfterFiveMinutes_QueriesRepositoryAgain()
	{
		SetupGames("event-1", ModelFactory.CreateGame("g1"));
		var service = CreateService();

		await service.GetAllAsync("event-1");
		_timeProvider.Advance(TimeSpan.FromMinutes(6));
		await service.GetAllAsync("event-1");

		await _repository.Received(2).GetGamesAsync("event-1", Arg.Any<CancellationToken>());
	}

	[TestMethod]
	public async Task GetAllAsync_DifferentEvents_AreCachedSeparately()
	{
		SetupGames("event-1", ModelFactory.CreateGame("g1"));
		SetupGames("event-2", ModelFactory.CreateGame("g2"));
		var service = CreateService();

		var first = await service.GetAllAsync("event-1");
		var second = await service.GetAllAsync("event-2");

		Assert.AreEqual("g1", first.Single().Id);
		Assert.AreEqual("g2", second.Single().Id);
	}

	[TestMethod]
	public async Task GetAllAsync_FetchFailsAfterSuccess_ReturnsLastKnownGames()
	{
		SetupGames("event-1", ModelFactory.CreateGame("g1"));
		var service = CreateService();
		await service.GetAllAsync("event-1");

		_timeProvider.Advance(TimeSpan.FromMinutes(10));
		_repository.GetGamesAsync("event-1", Arg.Any<CancellationToken>()).Returns(ResultFactory.Failure<Game>());
		var result = await service.GetAllAsync("event-1");

		Assert.AreEqual("g1", result.Single().Id);
	}

	[TestMethod]
	public async Task GetAllAsync_FirstFetchFails_ReturnsEmptyList()
	{
		_repository.GetGamesAsync("event-1", Arg.Any<CancellationToken>()).Returns(ResultFactory.Failure<Game>());
		var service = CreateService();

		var result = await service.GetAllAsync("event-1");

		Assert.IsEmpty(result);
	}

	[TestMethod]
	public async Task GetBuildingsAsync_SeveralTablesInOneBuilding_ReturnsBuildingOnce()
	{
		SetupGames("event-1",
			ModelFactory.CreateGame("g1", "Main", "Main"),
			ModelFactory.CreateGame("g2", "Main"));
		var service = CreateService();

		var result = await service.GetBuildingsAsync("event-1");

		CollectionAssert.AreEqual(new[] { "Main" }, result.ToArray());
	}

	[TestMethod]
	public async Task GetBuildingsAsync_DifferentBuildings_ReturnsEachBuilding()
	{
		SetupGames("event-1",
			ModelFactory.CreateGame("g1", "Main"),
			ModelFactory.CreateGame("g2", "Annex"));
		var service = CreateService();

		var result = await service.GetBuildingsAsync("event-1");

		CollectionAssert.AreEquivalent(new[] { "Main", "Annex" }, result.ToArray());
	}

	[TestMethod]
	public async Task GetBuildingsAsync_TablesWithoutLocation_AreSkipped()
	{
		SetupGames("event-1",
			ModelFactory.CreateGame("g1", null, "Main"),
			ModelFactory.CreateGame("g2", (string?)null));
		var service = CreateService();

		var result = await service.GetBuildingsAsync("event-1");

		CollectionAssert.AreEqual(new[] { "Main" }, result.ToArray());
	}

	[TestMethod]
	public async Task GetBuildingsAsync_NoGames_ReturnsEmptyList()
	{
		SetupGames("event-1");
		var service = CreateService();

		var result = await service.GetBuildingsAsync("event-1");

		Assert.IsEmpty(result);
	}
}
