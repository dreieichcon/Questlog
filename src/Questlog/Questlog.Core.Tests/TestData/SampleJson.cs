namespace Questlog.Core.Tests.TestData;

public static class SampleJson
{
	public const string Conventions = """
		[
			{
				"id": "00000000-0000-0000-0000-000000000001",
				"name": "Testcon 2030",
				"slug": "testcon30",
				"location": "",
				"begin_convention": "2030-11-14T09:00:00.000000Z",
				"end_convention": "2030-11-15T17:00:00.000000Z",
				"begin_show": "2030-09-03T10:00:00.000000Z",
				"end_show": "",
				"description_overview": "Line one\n\nLine two",
				"module_game": true,
				"module_exhibitor": false,
				"module_programm": true,
				"logo": "/storage/test/logo.png"
			}
		]
		""";

	public const string Games = """
		[
			{
				"id": "00000000-0000-0000-0000-0000000000a1",
				"title": "Test Adventure",
				"system": "Test System",
				"system_version": "",
				"chars": "Test heroes",
				"content_note": null,
				"teaser": "A short teaser",
				"start": "2030-11-14T09:30:00.000000Z",
				"duration": 300,
				"player_min": 2,
				"player_max": 6,
				"players_count": 3,
				"has_free_slots": true,
				"age_min": 14,
				"age_max": null,
				"game_master": "Test Master",
				"language": { "name": "Deutsch" },
				"tags": [ { "name": "Tag One" }, { "name": "Tag Two" } ],
				"tables": [ { "name": "Table-01" } ]
			}
		]
		""";

	public const string GameWithNullValues = """
		[
			{
				"id": "00000000-0000-0000-0000-0000000000a2",
				"title": "Null Adventure",
				"system": "Test System",
				"system_version": null,
				"start": "2030-11-14T09:30:00.000000Z",
				"duration": 60,
				"player_max": 4,
				"players_count": null,
				"age_min": null,
				"game_master": "Test Master",
				"tables": []
			}
		]
		""";

	public const string GameWithTableDetails = """
		[
			{
				"id": "00000000-0000-0000-0000-0000000000a3",
				"title": "Detail Adventure",
				"system": "Test System",
				"start": "2030-11-14T09:30:00.000000Z",
				"duration": 60,
				"player_max": 4,
				"game_master": "Test Master",
				"tables": [
					{
						"name": "Table-02",
						"wheelchair_accessible": true,
						"location": { "table": "2", "room": "Room A", "building": "Main" }
					}
				]
			}
		]
		""";

	public const string ProgramItems = """
		[
			{
				"id": "00000000-0000-0000-0000-0000000000b1",
				"title": "Test Reading",
				"host": "Test Host",
				"teaser": "A short teaser",
				"content_note": "",
				"start": "2030-11-14T10:00:00.000000Z",
				"duration": 30,
				"age_min": 12,
				"age_max": null,
				"language": { "name": "Deutsch" },
				"tags": [ { "name": "Lesung" }, { "name": "Zweites" } ],
				"tables": [ { "name": "Reading Room" }, { "name": "Second Room" } ]
			}
		]
		""";
}
