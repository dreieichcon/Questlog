using Questlog.Core.Models;

namespace Questlog.TestData;

public static class ModelFactory
{
	public static Convention CreateConvention(string id)
		=> new() { Id = id, Name = $"Name {id}" };

	public static Table CreateTable(string? building = null)
		=> new()
		{
			Name = "Table",
			Location = building is null ? null : new Location { Table = "1", Room = "Room", Building = building },
		};

	public static Game CreateGame(string id, params string?[] buildings)
		=> new()
		{
			Id = id,
			Title = $"Title {id}",
			GameMaster = "Master",
			Tables = buildings.Select(CreateTable).ToList(),
		};

	public static ProgramItem CreateProgramItem(string id)
		=> new() { Id = id };
}
