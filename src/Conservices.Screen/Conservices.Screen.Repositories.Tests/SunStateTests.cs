using Conservices.Screen.Models.Response;
using Conservices.Screen.Repositories.Serialization.Sun;
using Conservices.Screen.Repositories.Tests.TestData;

namespace Conservices.Screen.Repositories.Tests;

[TestClass]
public class SunStateTests
{
	private readonly SunStateSerializer _serializer = new();

	[TestMethod]
	public void Deserialize_SunStateResponse_ReadsTimes()
	{
		var result = _serializer.Deserialize<SunStateResponse>(SampleJson.SunState)!.SunState;

		Assert.AreEqual(new DateOnly(2030, 6, 21), result.Date);
		Assert.AreEqual(new TimeOnly(3, 30), result.Sunrise);
		Assert.AreEqual(new TimeOnly(19, 45), result.Sunset);
		Assert.AreEqual(new TimeOnly(20, 15), result.Dusk);
		Assert.AreEqual(TimeSpan.FromHours(16.25), result.DayLength);
	}

	[TestMethod]
	public void Deserialize_SunStateResponse_ReadsTimezone()
	{
		var result = _serializer.Deserialize<SunStateResponse>(SampleJson.SunState)!.SunState;

		Assert.AreEqual("UTC", result.Timezone);
		Assert.AreEqual(0, result.UtcOffset);
	}
}
