namespace Questlog.TestData;

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

	public const string SunState = """
		{"results":{"date":"2026-10-25","sunrise":"06:00:43","sunset":"16:17:24","first_light":"04:12:16","last_light":"18:05:39","dawn":"05:27:27","dusk":"16:50:37","solar_noon":"11:09:26","golden_hour":"15:31:28","day_length":"10:16:40","nautical_twilight_begin":"04:49:42","nautical_twilight_end":"17:28:19","timezone":"UTC","utc_offset":0,"sun_altitude":27.82,"sun_azimuth":180,"sunrise_azimuth":107.52,"sunset_azimuth":252.24,"moonrise":"15:29:29","moonset":"05:08:40","moon_illumination":99.26,"moon_phase":"Full Moon","moon_phase_value":0.47,"moon_always_up":false,"moon_always_down":false,"elevation":132,"sun_status":"normal","golden_hour_morning":{"begin":"05:40:13","end":"06:46:41"},"golden_hour_evening":{"begin":"15:31:28","end":"16:37:52"},"blue_hour_morning":{"begin":"05:27:27","end":"05:40:13"},"blue_hour_evening":{"begin":"16:37:52","end":"16:50:37"}},"status":"OK","tzid":"UTC"}
		""";

	public const string SunStateInvalidRequest = """
		{"results":[],"status":"INVALID_REQUEST","tzid":"UTC"}
		""";
}
