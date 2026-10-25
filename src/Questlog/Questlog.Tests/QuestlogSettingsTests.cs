using Microsoft.Extensions.Configuration;
using Questlog.Configuration;

namespace Questlog.Tests;

[TestClass]
public class QuestlogSettingsTests
{
	private const double DefaultLatitude = 50.02485985471239;

	private const double DefaultLongitude = 8.66270997993345;

	private static QuestlogSettings Read(params (string Key, string? Value)[] values)
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(values.Select(x => new KeyValuePair<string, string?>(x.Key, x.Value)))
			.Build();

		return QuestlogSettings.FromConfiguration(configuration);
	}

	[TestMethod]
	public void FromConfiguration_NothingSet_UsesBerlinAndDreieich()
	{
		var settings = Read();

		Assert.AreEqual("Europe/Berlin", settings.TimeZone.Id);
		Assert.AreEqual(DefaultLatitude, settings.Location.Latitude);
		Assert.AreEqual(DefaultLongitude, settings.Location.Longitude);
	}

	[TestMethod]
	public void FromConfiguration_ValidTimeZone_IsUsed()
	{
		var settings = Read(("QUESTLOG_TZ", "America/New_York"));

		Assert.AreEqual("America/New_York", settings.TimeZone.Id);
	}

	[TestMethod]
	[DataRow("")]
	[DataRow("   ")]
	[DataRow("Not/AZone")]
	public void FromConfiguration_MissingOrInvalidTimeZone_FallsBackToBerlin(string value)
	{
		var settings = Read(("QUESTLOG_TZ", value));

		Assert.AreEqual("Europe/Berlin", settings.TimeZone.Id);
	}

	[TestMethod]
	public void FromConfiguration_ValidCoordinates_AreUsedWithInvariantCulture()
	{
		var settings = Read(("QUESTLOG_LAT", "48.1371"), ("QUESTLOG_LNG", "-11.5754"));

		Assert.AreEqual(48.1371, settings.Location.Latitude);
		Assert.AreEqual(-11.5754, settings.Location.Longitude);
	}

	[TestMethod]
	[DataRow("abc", "8.6")]
	[DataRow("50.0", "abc")]
	[DataRow("91", "8.6")]
	[DataRow("-91", "8.6")]
	[DataRow("50.0", "181")]
	[DataRow("50.0", "-181")]
	[DataRow("50.0", null)]
	[DataRow(null, "8.6")]
	public void FromConfiguration_InvalidOrIncompleteCoordinates_FallBackToDreieichPair(string? latitude, string? longitude)
	{
		var settings = Read(("QUESTLOG_LAT", latitude), ("QUESTLOG_LNG", longitude));

		Assert.AreEqual(DefaultLatitude, settings.Location.Latitude);
		Assert.AreEqual(DefaultLongitude, settings.Location.Longitude);
	}
}
