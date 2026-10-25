using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Services.Tests.Helpers;
using Questlog.TestData;

namespace Questlog.Services.Tests;

[TestClass]
public class ConventionServiceTests
{
	private readonly IConservicesRepository _repository = Substitute.For<IConservicesRepository>();

	private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2030, 11, 14, 10, 0, 0, TimeSpan.Zero));

	private ConventionService CreateService() => new(_repository, _timeProvider);

	private void SetupConventions(params string[] ids)
		=> _repository.GetConventionsAsync(Arg.Any<CancellationToken>())
			.Returns(ResultFactory.Success(ids.Select(ModelFactory.CreateConvention).ToArray()));

	[TestMethod]
	public async Task GetAllAsync_ReturnsConventionsOfRepository()
	{
		SetupConventions("a", "b");
		var service = CreateService();

		var result = await service.GetAllAsync();

		CollectionAssert.AreEqual(new[] { "a", "b" }, result.Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public async Task GetAllAsync_WithinOneHour_QueriesRepositoryOnce()
	{
		SetupConventions("a");
		var service = CreateService();

		await service.GetAllAsync();
		_timeProvider.Advance(TimeSpan.FromMinutes(59));
		await service.GetAllAsync();

		await _repository.Received(1).GetConventionsAsync(Arg.Any<CancellationToken>());
	}

	[TestMethod]
	public async Task GetAllAsync_AfterOneHour_QueriesRepositoryAgain()
	{
		SetupConventions("a");
		var service = CreateService();

		await service.GetAllAsync();
		_timeProvider.Advance(TimeSpan.FromMinutes(61));
		await service.GetAllAsync();

		await _repository.Received(2).GetConventionsAsync(Arg.Any<CancellationToken>());
	}

	[TestMethod]
	public async Task GetAsync_KnownId_ReturnsMatchingConvention()
	{
		SetupConventions("a", "b");
		var service = CreateService();

		var result = await service.GetAsync("b");

		Assert.AreEqual("b", result?.Id);
	}

	[TestMethod]
	public async Task GetAsync_UnknownId_ReturnsNull()
	{
		SetupConventions("a");
		var service = CreateService();

		var result = await service.GetAsync("unknown");

		Assert.IsNull(result);
	}

	[TestMethod]
	public async Task GetAsync_AfterGetAllAsync_QueriesRepositoryOnce()
	{
		SetupConventions("a");
		var service = CreateService();

		await service.GetAllAsync();
		await service.GetAsync("a");

		await _repository.Received(1).GetConventionsAsync(Arg.Any<CancellationToken>());
	}

	[TestMethod]
	public async Task GetAllAsync_FirstFetchFails_ReturnsEmptyListAndRetriesNextCall()
	{
		_repository.GetConventionsAsync(Arg.Any<CancellationToken>()).Returns(ResultFactory.Failure<Core.Models.Convention>());
		var service = CreateService();

		var first = await service.GetAllAsync();
		var second = await service.GetAllAsync();

		Assert.IsEmpty(first);
		Assert.IsEmpty(second);
		await _repository.Received(2).GetConventionsAsync(Arg.Any<CancellationToken>());
	}

	[TestMethod]
	public async Task GetAllAsync_FetchFailsAfterSuccess_ReturnsLastKnownData()
	{
		SetupConventions("a");
		var service = CreateService();
		await service.GetAllAsync();

		_timeProvider.Advance(TimeSpan.FromHours(2));
		_repository.GetConventionsAsync(Arg.Any<CancellationToken>()).Returns(ResultFactory.Failure<Core.Models.Convention>());
		var result = await service.GetAllAsync();

		Assert.AreEqual("a", result.Single().Id);
	}

	[TestMethod]
	public async Task GetAllAsync_FailureIsNotCached_NextCallFetchesAndReplacesStaleData()
	{
		SetupConventions("a");
		var service = CreateService();
		await service.GetAllAsync();

		_timeProvider.Advance(TimeSpan.FromHours(2));
		_repository.GetConventionsAsync(Arg.Any<CancellationToken>()).Returns(ResultFactory.Failure<Core.Models.Convention>());
		await service.GetAllAsync();
		SetupConventions("b");
		var result = await service.GetAllAsync();

		Assert.AreEqual("b", result.Single().Id);
	}

	[TestMethod]
	public async Task GetAllAsync_ConcurrentCalls_ShareOneFetch()
	{
		var completion = new TaskCompletionSource<Demolite.Core.Models.Result<IReadOnlyList<Core.Models.Convention>>>();
		_repository.GetConventionsAsync(Arg.Any<CancellationToken>()).Returns(completion.Task);
		var service = CreateService();

		var calls = Enumerable.Range(0, 5).Select(_ => service.GetAllAsync()).ToArray();
		completion.SetResult(ResultFactory.Success(ModelFactory.CreateConvention("a")));
		var results = await Task.WhenAll(calls);

		await _repository.Received(1).GetConventionsAsync(Arg.Any<CancellationToken>());
		Assert.IsTrue(results.All(x => x.Single().Id == "a"));
	}
}
