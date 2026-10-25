using Conservices.Screen.Models.Convention;
using Conservices.Screen.Models.Games;
using Conservices.Screen.Models.Misc;
using Conservices.Screen.Models.Program;

namespace Conservices.Screen.Services.Tests.TestData;

public static class ModelFactory
{
	public static Convention CreateConvention(string id)
		=> new() { Id = id, Name = $"Name {id}", Slug = $"slug-{id}" };

	public static Table CreateTable(string building)
		=> new()
		{
			Name = "Table",
			Description = string.Empty,
			Location = new Location { Table = "1", Room = "Room", Building = building },
		};

	public static Game CreateGame(string id, params string[] buildings)
		=> new()
		{
			Id = id,
			Title = $"Title {id}",
			GameMaster = "Master",
			Language = new Language { Name = "Deutsch" },
			Tables = buildings.Select(CreateTable).ToList(),
		};

	public static ProgramItem CreateProgramItem(string id)
		=> new()
		{
			Id = id,
			Host = "Host",
			Language = new Language { Name = "Deutsch" },
		};
}
