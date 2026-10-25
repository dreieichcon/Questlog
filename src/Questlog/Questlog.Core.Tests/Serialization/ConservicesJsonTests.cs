using System.Text.Json;
using Questlog.Core.Models;
using Questlog.Core.Serialization;
using Questlog.TestData;

namespace Questlog.Core.Tests.Serialization;

[TestClass]
public class ConservicesJsonTests
{
	private static DateTimeOffset Utc(int year, int month, int day, int hour, int minute)
		=> new(year, month, day, hour, minute, 0, TimeSpan.Zero);

	private static T Parse<T>(string json)
		=> JsonSerializer.Deserialize<T>(json, ConservicesJson.Options)!;

	[TestMethod]
	public void Convention_ReadsUsedFields()
	{
		var result = Parse<List<Convention>>(SampleJson.Conventions).Single();

		Assert.AreEqual("00000000-0000-0000-0000-000000000001", result.Id);
		Assert.AreEqual("Testcon 2030", result.Name);
		Assert.AreEqual("/storage/test/logo.png", result.Logo);
	}

	[TestMethod]
	public void Convention_ReadsModuleFlags()
	{
		var result = Parse<List<Convention>>(SampleJson.Conventions).Single();

		Assert.IsTrue(result.HasGameModule);
		Assert.IsFalse(result.HasExhibitorModule);
		Assert.IsTrue(result.HasProgramModule);
	}

	[TestMethod]
	public void Convention_ReadsIsoTimestampsAsExactInstants()
	{
		var result = Parse<List<Convention>>(SampleJson.Conventions).Single();

		Assert.AreEqual(Utc(2030, 11, 14, 9, 0), result.ConventionStart);
		Assert.AreEqual(Utc(2030, 11, 15, 17, 0), result.ConventionEnd);
		Assert.AreEqual(TimeSpan.Zero, result.ConventionStart?.Offset);
	}

	[TestMethod]
	public void Game_ReadsUsedFields()
	{
		var result = Parse<List<Game>>(SampleJson.Games).Single();

		Assert.AreEqual("Test Adventure", result.Title);
		Assert.AreEqual("Test System", result.System);
		Assert.AreEqual("Test Master", result.GameMaster);
		Assert.AreEqual(6, result.PlayerMax);
		Assert.AreEqual(3, result.PlayerCount);
		Assert.AreEqual(14, result.AgeMin);
	}

	[TestMethod]
	public void Game_ReadsStartAndComputesDurationAndEnd()
	{
		var result = Parse<List<Game>>(SampleJson.Games).Single();

		Assert.AreEqual(Utc(2030, 11, 14, 9, 30), result.Start);
		Assert.AreEqual(TimeSpan.FromHours(5), result.Duration);
		Assert.AreEqual(Utc(2030, 11, 14, 14, 30), result.End);
	}

	[TestMethod]
	public void Game_NullOrEmptyOptionalValues_AreReadWithoutError()
	{
		var result = Parse<List<Game>>(SampleJson.GameWithNullValues).Single();

		Assert.IsNull(result.SystemVersion);
		Assert.IsNull(result.PlayerCount);
		Assert.IsNull(result.AgeMin);
		Assert.IsEmpty(result.Tables);
	}

	[TestMethod]
	public void Game_TableWithNameOnly_IsNotAccessibleAndHasNoLocation()
	{
		var result = Parse<List<Game>>(SampleJson.Games).Single();

		var table = result.Tables.Single();

		Assert.AreEqual("Table-01", table.Name);
		Assert.IsFalse(table.Accessible);
		Assert.IsNull(table.Location);
	}

	[TestMethod]
	public void Game_TableWithDetails_ReadsAccessibilityAndLocation()
	{
		var result = Parse<List<Game>>(SampleJson.GameWithTableDetails).Single();

		var table = result.Tables.Single();

		Assert.IsTrue(table.Accessible);
		Assert.AreEqual("Main", table.Location?.Building);
		Assert.AreEqual("Room A", table.Location?.Room);
	}

	[TestMethod]
	public void ProgramItem_ReadsUsedFields()
	{
		var result = Parse<List<ProgramItem>>(SampleJson.ProgramItems).Single();

		Assert.AreEqual("Test Reading", result.Title);
		Assert.AreEqual(12, result.AgeMin);
		Assert.AreEqual(Utc(2030, 11, 14, 10, 0), result.Start);
		Assert.AreEqual(Utc(2030, 11, 14, 10, 30), result.End);
	}

	[TestMethod]
	public void ProgramItem_ExposesFirstTableAndFirstTag()
	{
		var result = Parse<List<ProgramItem>>(SampleJson.ProgramItems).Single();

		Assert.HasCount(2, result.Tables);
		Assert.AreEqual("Reading Room", result.Table?.Name);
		Assert.AreEqual("Lesung", result.Tag?.Name);
	}

	[TestMethod]
	public void ProgramItem_WithoutTablesAndTags_HasNoFirstTableOrTag()
	{
		var item = new ProgramItem { Id = "1" };

		Assert.IsNull(item.Table);
		Assert.IsNull(item.Tag);
	}

	[TestMethod]
	public void NullableTimestamp_EmptyString_IsReadAsNull()
	{
		var result = JsonSerializer.Deserialize<DateTimeOffset?>("\"\"", ConservicesJson.Options);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void NullableTimestamp_NullValue_IsReadAsNull()
	{
		var result = JsonSerializer.Deserialize<DateTimeOffset?>("null", ConservicesJson.Options);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void NullableTimestamp_OtherOffset_IsKeptAsSameInstant()
	{
		var result = JsonSerializer.Deserialize<DateTimeOffset?>("\"2030-11-14T10:00:00+01:00\"", ConservicesJson.Options);

		Assert.AreEqual(Utc(2030, 11, 14, 9, 0), result);
	}

	[TestMethod]
	public void NullableTimestamp_Unparsable_IsReadAsNull()
	{
		var result = JsonSerializer.Deserialize<DateTimeOffset?>("\"not a date\"", ConservicesJson.Options);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void NullableTimestamp_WrittenAsIsoUtc()
	{
		var json = JsonSerializer.Serialize<DateTimeOffset?>(Utc(2030, 11, 14, 9, 0), ConservicesJson.Options);

		Assert.AreEqual("\"2030-11-14T09:00:00+00:00\"", json);
	}
}
