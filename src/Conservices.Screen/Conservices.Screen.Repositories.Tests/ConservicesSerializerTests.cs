using Conservices.Screen.Models.Convention;
using Conservices.Screen.Models.Games;
using Conservices.Screen.Models.Program;
using Conservices.Screen.Repositories.Serialization.Conservices;
using Conservices.Screen.Repositories.Tests.TestData;

namespace Conservices.Screen.Repositories.Tests;

[TestClass]
public class ConservicesSerializerTests
{
	private readonly ConservicesSerializer _serializer = new();

	private static DateTime Utc(int year, int month, int day, int hour, int minute)
		=> new(year, month, day, hour, minute, 0, DateTimeKind.Utc);

	[TestMethod]
	public void Deserialize_Convention_ReadsBasicFields()
	{
		var result = _serializer.Deserialize<List<Convention>>(SampleJson.Conventions)!.Single();

		Assert.AreEqual("00000000-0000-0000-0000-000000000001", result.Id);
		Assert.AreEqual("Testcon 2030", result.Name);
		Assert.AreEqual("testcon30", result.Slug);
		Assert.AreEqual("Line one\n\nLine two", result.Description);
		Assert.AreEqual("/storage/test/logo.png", result.Logo);
	}

	[TestMethod]
	public void Deserialize_Convention_ReadsModuleSwitches()
	{
		var result = _serializer.Deserialize<List<Convention>>(SampleJson.Conventions)!.Single();

		Assert.IsTrue(result.HasGameModule);
		Assert.IsFalse(result.HasExhibitorModule);
		Assert.IsTrue(result.HasProgramModule);
	}

	[TestMethod]
	public void Deserialize_Convention_ReadsIsoTimestampsAsUtcInstants()
	{
		var result = _serializer.Deserialize<List<Convention>>(SampleJson.Conventions)!.Single();

		Assert.AreEqual(Utc(2030, 11, 14, 9, 0), result.ConventionStart?.ToUniversalTime());
		Assert.AreEqual(Utc(2030, 11, 15, 17, 0), result.ConventionEnd?.ToUniversalTime());
		Assert.AreEqual(Utc(2030, 9, 3, 10, 0), result.ShowStart?.ToUniversalTime());
	}

	[TestMethod]
	public void Deserialize_Convention_NullTimestamp_StaysNull()
	{
		var result = _serializer.Deserialize<List<Convention>>(SampleJson.Conventions)!.Single();

		Assert.IsNull(result.ShowEnd);
	}

	[TestMethod]
	public void Deserialize_Convention_EmptyLocation_IsReadAsEmptyString()
	{
		var result = _serializer.Deserialize<List<Convention>>(SampleJson.Conventions)!.Single();

		Assert.AreEqual(string.Empty, result.Location);
	}

	[TestMethod]
	public void Deserialize_Game_ReadsBasicFields()
	{
		var result = _serializer.Deserialize<List<Game>>(SampleJson.Games)!.Single();

		Assert.AreEqual("Test Adventure", result.Title);
		Assert.AreEqual("Test System", result.System);
		Assert.AreEqual("Test heroes", result.Characters);
		Assert.AreEqual("Test Master", result.GameMaster);
		Assert.AreEqual("Deutsch", result.Language.Name);
		Assert.IsTrue(result.HasFreeSlots);
	}

	[TestMethod]
	public void Deserialize_Game_ReadsPlayerAndAgeLimits()
	{
		var result = _serializer.Deserialize<List<Game>>(SampleJson.Games)!.Single();

		Assert.AreEqual(2, result.PlayerMin);
		Assert.AreEqual(6, result.PlayerMax);
		Assert.AreEqual(3, result.PlayerCount);
		Assert.AreEqual(14, result.AgeMin);
		Assert.IsNull(result.AgeMax);
	}

	[TestMethod]
	public void Deserialize_Game_ReadsStartAsUtcInstantAndDurationInMinutes()
	{
		var result = _serializer.Deserialize<List<Game>>(SampleJson.Games)!.Single();

		Assert.AreEqual(Utc(2030, 11, 14, 9, 30), result.Start.ToUniversalTime());
		Assert.AreEqual(TimeSpan.FromHours(5), result.Duration);
		Assert.AreEqual(Utc(2030, 11, 14, 14, 30), result.End?.ToUniversalTime());
	}

	[TestMethod]
	public void Deserialize_Game_NullContentNote_StaysNull()
	{
		var result = _serializer.Deserialize<List<Game>>(SampleJson.Games)!.Single();

		Assert.IsNull(result.ContentNote);
	}

	[TestMethod]
	public void Deserialize_Game_ReadsTagsInOrder()
	{
		var result = _serializer.Deserialize<List<Game>>(SampleJson.Games)!.Single();

		CollectionAssert.AreEqual(new[] { "Tag One", "Tag Two" }, result.Tags.Select(x => x.Name).ToArray());
	}

	[TestMethod]
	public void Deserialize_Game_TableWithNameOnly_HasNoLocation()
	{
		var result = _serializer.Deserialize<List<Game>>(SampleJson.Games)!.Single();

		var table = result.Tables.Single();

		Assert.AreEqual("Table-01", table.Name);
		Assert.IsNull(table.Location);
	}

	[TestMethod]
	public void Deserialize_ProgramItem_ReadsBasicFields()
	{
		var result = _serializer.Deserialize<List<ProgramItem>>(SampleJson.ProgramItems)!.Single();

		Assert.AreEqual("Test Reading", result.Title);
		Assert.AreEqual("Test Host", result.Host);
		Assert.AreEqual(12, result.MinimumAge);
		Assert.IsNull(result.MaximumAge);
		Assert.AreEqual(string.Empty, result.ContentNote);
	}

	[TestMethod]
	public void Deserialize_ProgramItem_ReadsStartAndEnd()
	{
		var result = _serializer.Deserialize<List<ProgramItem>>(SampleJson.ProgramItems)!.Single();

		Assert.AreEqual(Utc(2030, 11, 14, 10, 0), result.Start.ToUniversalTime());
		Assert.AreEqual(Utc(2030, 11, 14, 10, 30), result.End.ToUniversalTime());
	}

	[TestMethod]
	public void Deserialize_ProgramItem_ExposesFirstTableAndLabel()
	{
		var result = _serializer.Deserialize<List<ProgramItem>>(SampleJson.ProgramItems)!.Single();

		Assert.AreEqual("Reading Room", result.Table?.Name);
		Assert.AreEqual("Lesung", result.Label?.Name);
	}
}
