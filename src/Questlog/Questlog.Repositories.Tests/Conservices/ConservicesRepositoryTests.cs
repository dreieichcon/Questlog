using System.Net;
using Questlog.Repositories.Conservices;
using Questlog.Repositories.Tests.Helpers;
using Questlog.TestData;

namespace Questlog.Repositories.Tests.Conservices;

[TestClass]
public class ConservicesRepositoryTests
{
	private static ConservicesRepository CreateRepository(FakeHandler handler)
	{
		var client = new HttpClient(handler) { BaseAddress = new Uri(ConservicesRepository.BaseAddress) };
		return new ConservicesRepository(client);
	}

	[TestMethod]
	public async Task GetConventionsAsync_CallsConventionEndpoint()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, SampleJson.Conventions);

		await CreateRepository(handler).GetConventionsAsync();

		var request = handler.Requests.Single();
		Assert.AreEqual(HttpMethod.Get, request.Method);
		Assert.AreEqual("https://conservices.de/api/convention", request.RequestUri?.ToString());
	}

	[TestMethod]
	public async Task GetGamesAsync_CallsGamesEndpointOfEvent()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, SampleJson.Games);

		await CreateRepository(handler).GetGamesAsync("event-1");

		Assert.AreEqual("https://conservices.de/api/convention/event-1/games", handler.Requests.Single().RequestUri?.ToString());
	}

	[TestMethod]
	public async Task GetProgramItemsAsync_CallsProgramEndpointOfEvent()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, SampleJson.ProgramItems);

		await CreateRepository(handler).GetProgramItemsAsync("event-1");

		Assert.AreEqual("https://conservices.de/api/convention/event-1/programms", handler.Requests.Single().RequestUri?.ToString());
	}

	[TestMethod]
	public async Task GetGamesAsync_EventIdWithSpecialCharacters_IsEscaped()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, "[]");

		await CreateRepository(handler).GetGamesAsync("a/b c");

		Assert.AreEqual("https://conservices.de/api/convention/a%2Fb%20c/games", handler.Requests.Single().RequestUri?.AbsoluteUri);
	}

	[TestMethod]
	public async Task Requests_AskForJson()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, "[]");

		await CreateRepository(handler).GetConventionsAsync();

		var accept = handler.Requests.Single().Headers.Accept;
		Assert.AreEqual("application/json", accept.Single().MediaType);
	}

	[TestMethod]
	public async Task GetConventionsAsync_ValidResponse_ReturnsParsedConventions()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, SampleJson.Conventions);

		var result = await CreateRepository(handler).GetConventionsAsync();

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual("Testcon 2030", result.Value!.Single().Name);
	}

	[TestMethod]
	public async Task GetGamesAsync_ValidResponse_ReturnsParsedGames()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, SampleJson.Games);

		var result = await CreateRepository(handler).GetGamesAsync("event-1");

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual("Test Adventure", result.Value!.Single().Title);
	}

	[TestMethod]
	public async Task GetProgramItemsAsync_ValidResponse_ReturnsParsedItems()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, SampleJson.ProgramItems);

		var result = await CreateRepository(handler).GetProgramItemsAsync("event-1");

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual("Test Reading", result.Value!.Single().Title);
	}

	[TestMethod]
	public async Task GetGamesAsync_EmptyList_IsSuccessWithoutItems()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, "[]");

		var result = await CreateRepository(handler).GetGamesAsync("event-1");

		Assert.IsTrue(result.IsSuccess);
		Assert.IsEmpty(result.Value!);
	}

	[TestMethod]
	[DataRow(HttpStatusCode.InternalServerError)]
	[DataRow(HttpStatusCode.NotFound)]
	[DataRow(HttpStatusCode.Forbidden)]
	public async Task GetGamesAsync_ErrorStatus_ReturnsFailure(HttpStatusCode status)
	{
		var handler = FakeHandler.Responding(status, "error");

		var result = await CreateRepository(handler).GetGamesAsync("event-1");

		Assert.IsFalse(result.IsSuccess);
		Assert.AreEqual($"Http.{(int)status}", result.Error?.Code);
	}

	[TestMethod]
	public async Task GetGamesAsync_ConnectionError_ReturnsFailure()
	{
		var handler = FakeHandler.Throwing(new HttpRequestException("no connection"));

		var result = await CreateRepository(handler).GetGamesAsync("event-1");

		Assert.IsFalse(result.IsSuccess);
	}

	[TestMethod]
	public async Task GetGamesAsync_InvalidJson_ReturnsFailure()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, "<html>not json</html>");

		var result = await CreateRepository(handler).GetGamesAsync("event-1");

		Assert.IsFalse(result.IsSuccess);
	}

	[TestMethod]
	[DataRow("")]
	[DataRow("null")]
	public async Task GetGamesAsync_EmptyBody_ReturnsFailure(string body)
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, body);

		var result = await CreateRepository(handler).GetGamesAsync("event-1");

		Assert.IsFalse(result.IsSuccess);
	}
}
