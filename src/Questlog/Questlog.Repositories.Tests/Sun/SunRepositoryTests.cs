using System.Net;
using Questlog.Core.Configuration;
using Questlog.Repositories.Sun;
using Questlog.Repositories.Tests.Helpers;
using Questlog.TestData;

namespace Questlog.Repositories.Tests.Sun;

[TestClass]
public class SunRepositoryTests
{
	private static SunRepository CreateRepository(FakeHandler handler)
	{
		var client = new HttpClient(handler) { BaseAddress = new Uri(SunRepository.BaseAddress) };
		return new SunRepository(client, new GeoLocation(50.5, 8.25));
	}

	[TestMethod]
	public async Task GetSunStateAsync_RequestsUtcTimesInTwentyFourHourFormatForConfiguredLocation()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, SampleJson.SunState);

		await CreateRepository(handler).GetSunStateAsync();

		var request = handler.Requests.Single();
		Assert.AreEqual(HttpMethod.Get, request.Method);
		Assert.AreEqual("https://api.sunrisesunset.io/json?lat=50.5&lng=8.25&timezone=UTC&time_format=24", request.RequestUri?.ToString());
	}

	[TestMethod]
	public async Task GetSunStateAsync_ValidResponse_ReturnsDateSunriseAndSunset()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, SampleJson.SunState);

		var result = await CreateRepository(handler).GetSunStateAsync();

		Assert.IsTrue(result.IsSuccess);
		Assert.AreEqual(new DateOnly(2026, 10, 25), result.Value!.Date);
		Assert.AreEqual(new TimeOnly(6, 0, 43), result.Value.Sunrise);
		Assert.AreEqual(new TimeOnly(16, 17, 24), result.Value.Sunset);
	}

	[TestMethod]
	public async Task GetSunStateAsync_ApiReportsInvalidRequest_ReturnsFailure()
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, SampleJson.SunStateInvalidRequest);

		var result = await CreateRepository(handler).GetSunStateAsync();

		Assert.IsFalse(result.IsSuccess);
	}

	[TestMethod]
	[DataRow(HttpStatusCode.InternalServerError)]
	[DataRow(HttpStatusCode.TooManyRequests)]
	public async Task GetSunStateAsync_ErrorStatus_ReturnsFailure(HttpStatusCode status)
	{
		var handler = FakeHandler.Responding(status, "error");

		var result = await CreateRepository(handler).GetSunStateAsync();

		Assert.IsFalse(result.IsSuccess);
	}

	[TestMethod]
	public async Task GetSunStateAsync_ConnectionError_ReturnsFailure()
	{
		var handler = FakeHandler.Throwing(new HttpRequestException("no connection"));

		var result = await CreateRepository(handler).GetSunStateAsync();

		Assert.IsFalse(result.IsSuccess);
	}

	[TestMethod]
	[DataRow("")]
	[DataRow("{}")]
	[DataRow("not json")]
	public async Task GetSunStateAsync_UnusableBody_ReturnsFailure(string body)
	{
		var handler = FakeHandler.Responding(HttpStatusCode.OK, body);

		var result = await CreateRepository(handler).GetSunStateAsync();

		Assert.IsFalse(result.IsSuccess);
	}
}
