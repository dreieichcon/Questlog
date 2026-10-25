using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Services.Tests.Helpers;
using Questlog.TestData;

namespace Questlog.Services.Tests;

[TestClass]
public class ProgramServiceTests
{
	private readonly IConservicesRepository _repository = Substitute.For<IConservicesRepository>();

	private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2030, 11, 14, 10, 0, 0, TimeSpan.Zero));

	private ProgramService CreateService() => new(_repository, _timeProvider);

	private void SetupItems(string eventId, params ProgramItem[] items)
		=> _repository.GetProgramItemsAsync(eventId, Arg.Any<CancellationToken>()).Returns(ResultFactory.Success(items));

	[TestMethod]
	public async Task GetAllAsync_ReturnsItemsOfRequestedEvent()
	{
		SetupItems("event-1", ModelFactory.CreateProgramItem("p1"), ModelFactory.CreateProgramItem("p2"));
		var service = CreateService();

		var result = await service.GetAllAsync("event-1");

		CollectionAssert.AreEqual(new[] { "p1", "p2" }, result.Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public async Task GetAllAsync_WithinFiveMinutes_QueriesRepositoryOnce()
	{
		SetupItems("event-1", ModelFactory.CreateProgramItem("p1"));
		var service = CreateService();

		await service.GetAllAsync("event-1");
		_timeProvider.Advance(TimeSpan.FromMinutes(4));
		await service.GetAllAsync("event-1");

		await _repository.Received(1).GetProgramItemsAsync("event-1", Arg.Any<CancellationToken>());
	}

	[TestMethod]
	public async Task GetAllAsync_AfterFiveMinutes_QueriesRepositoryAgain()
	{
		SetupItems("event-1", ModelFactory.CreateProgramItem("p1"));
		var service = CreateService();

		await service.GetAllAsync("event-1");
		_timeProvider.Advance(TimeSpan.FromMinutes(6));
		await service.GetAllAsync("event-1");

		await _repository.Received(2).GetProgramItemsAsync("event-1", Arg.Any<CancellationToken>());
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

	[TestMethod]
	public async Task GetAllAsync_FetchFailsAfterSuccess_ReturnsLastKnownItems()
	{
		SetupItems("event-1", ModelFactory.CreateProgramItem("p1"));
		var service = CreateService();
		await service.GetAllAsync("event-1");

		_timeProvider.Advance(TimeSpan.FromMinutes(10));
		_repository.GetProgramItemsAsync("event-1", Arg.Any<CancellationToken>()).Returns(ResultFactory.Failure<ProgramItem>());
		var result = await service.GetAllAsync("event-1");

		Assert.AreEqual("p1", result.Single().Id);
	}

	[TestMethod]
	public async Task GetAllAsync_FirstFetchFails_ReturnsEmptyList()
	{
		_repository.GetProgramItemsAsync("event-1", Arg.Any<CancellationToken>()).Returns(ResultFactory.Failure<ProgramItem>());
		var service = CreateService();

		var result = await service.GetAllAsync("event-1");

		Assert.IsEmpty(result);
	}
}
