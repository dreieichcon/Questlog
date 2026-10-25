using Conservices.Screen.Interfaces.Repositories;
using Conservices.Screen.Services.Conservices;
using Conservices.Screen.Services.Tests.TestData;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace Conservices.Screen.Services.Tests;

[TestClass]
public class ConventionServiceTests
{
	private readonly IConventionRepository _repository = Substitute.For<IConventionRepository>();

	private readonly MemoryCache _cache = new(new MemoryCacheOptions());

	private ConventionService CreateService()
	{
		var conventions = new[] { ModelFactory.CreateConvention("a"), ModelFactory.CreateConvention("b") };
		_repository.GetAllAsync().Returns(conventions);

		return new ConventionService(_repository, _cache);
	}

	[TestMethod]
	public async Task GetAllAsync_ReturnsAllConventionsOfRepository()
	{
		var service = CreateService();

		var result = await service.GetAllAsync();

		CollectionAssert.AreEqual(new[] { "a", "b" }, result.Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public async Task GetAllAsync_CalledTwice_QueriesRepositoryOnce()
	{
		var service = CreateService();

		await service.GetAllAsync();
		await service.GetAllAsync();

		await _repository.Received(1).GetAllAsync();
	}

	[TestMethod]
	public async Task GetAsync_KnownId_ReturnsMatchingConvention()
	{
		var service = CreateService();

		var result = await service.GetAsync("b");

		Assert.AreEqual("b", result?.Id);
	}

	[TestMethod]
	public async Task GetAsync_UnknownId_ReturnsNull()
	{
		var service = CreateService();

		var result = await service.GetAsync("unknown");

		Assert.IsNull(result);
	}

	[TestMethod]
	public async Task GetAsync_AfterGetAllAsync_QueriesRepositoryOnce()
	{
		var service = CreateService();

		await service.GetAllAsync();
		await service.GetAsync("a");

		await _repository.Received(1).GetAllAsync();
	}
}
