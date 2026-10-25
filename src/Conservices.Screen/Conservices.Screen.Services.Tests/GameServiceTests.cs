using Conservices.Screen.Interfaces.Repositories;
using Conservices.Screen.Models.Games;
using Conservices.Screen.Services.Conservices;
using Conservices.Screen.Services.Tests.TestData;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace Conservices.Screen.Services.Tests;

[TestClass]
public class GameServiceTests
{
	private readonly IGameRepository _repository = Substitute.For<IGameRepository>();

	private readonly MemoryCache _cache = new(new MemoryCacheOptions());

	private GameService CreateService() => new(_repository, _cache);

	private void SetupGames(string eventId, params Game[] games)
		=> _repository.GetAllAsync(eventId).Returns(games);

	[TestMethod]
	public async Task GetAllAsync_ReturnsGamesOfRequestedEvent()
	{
		SetupGames("event-1", ModelFactory.CreateGame("g1"), ModelFactory.CreateGame("g2"));
		var service = CreateService();

		var result = await service.GetAllAsync("event-1");

		CollectionAssert.AreEqual(new[] { "g1", "g2" }, result.Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public async Task GetAllAsync_SameEventTwice_QueriesRepositoryOnce()
	{
		SetupGames("event-1", ModelFactory.CreateGame("g1"));
		var service = CreateService();

		await service.GetAllAsync("event-1");
		await service.GetAllAsync("event-1");

		await _repository.Received(1).GetAllAsync("event-1");
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
	public async Task GetAllBuildings_SeveralTablesInOneBuilding_ReturnsBuildingOnce()
	{
		SetupGames("event-1",
			ModelFactory.CreateGame("g1", "Main", "Main"),
			ModelFactory.CreateGame("g2", "Main"));
		var service = CreateService();

		var result = await service.GetAllBuildings("event-1");

		Assert.AreEqual("Main", result.Single().Building);
	}

	[TestMethod]
	public async Task GetAllBuildings_DifferentBuildings_ReturnsEachBuilding()
	{
		SetupGames("event-1",
			ModelFactory.CreateGame("g1", "Main"),
			ModelFactory.CreateGame("g2", "Annex"));
		var service = CreateService();

		var result = await service.GetAllBuildings("event-1");

		CollectionAssert.AreEquivalent(new[] { "Main", "Annex" }, result.Select(x => x.Building).ToArray());
	}

	[TestMethod]
	public async Task GetAllBuildings_NoGames_ReturnsEmptySequence()
	{
		SetupGames("event-1");
		var service = CreateService();

		var result = await service.GetAllBuildings("event-1");

		Assert.AreEqual(0, result.Count());
	}
}
