using Conservices.Screen.Interfaces.Repositories;
using Conservices.Screen.Models.Program;
using Conservices.Screen.Services.Conservices;
using Conservices.Screen.Services.Tests.TestData;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace Conservices.Screen.Services.Tests;

[TestClass]
public class ProgramServiceTests
{
	private readonly IProgramRepository _repository = Substitute.For<IProgramRepository>();

	private readonly MemoryCache _cache = new(new MemoryCacheOptions());

	private ProgramService CreateService() => new(_repository, _cache);

	private void SetupItems(string eventId, params ProgramItem[] items)
		=> _repository.GetAllAsync(eventId).Returns(items);

	[TestMethod]
	public async Task GetAllAsync_ReturnsItemsOfRequestedEvent()
	{
		SetupItems("event-1", ModelFactory.CreateProgramItem("p1"), ModelFactory.CreateProgramItem("p2"));
		var service = CreateService();

		var result = await service.GetAllAsync("event-1");

		CollectionAssert.AreEqual(new[] { "p1", "p2" }, result.Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public async Task GetAllAsync_SameEventTwice_QueriesRepositoryOnce()
	{
		SetupItems("event-1", ModelFactory.CreateProgramItem("p1"));
		var service = CreateService();

		await service.GetAllAsync("event-1");
		await service.GetAllAsync("event-1");

		await _repository.Received(1).GetAllAsync("event-1");
	}

	[TestMethod]
	public async Task GetAllAsync_DifferentEvents_AreCachedSeparately()
	{
		SetupItems("event-1", ModelFactory.CreateProgramItem("p1"));
		SetupItems("event-2", ModelFactory.CreateProgramItem("p2"));
		var service = CreateService();

		var first = await service.GetAllAsync("event-1");
		var second = await service.GetAllAsync("event-2");

		Assert.AreEqual("p1", first.Single().Id);
		Assert.AreEqual("p2", second.Single().Id);
	}
}
