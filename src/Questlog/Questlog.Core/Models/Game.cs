using System.Text.Json.Serialization;

namespace Questlog.Core.Models;

public class Game : IHasStartTime
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("title")]
	public required string Title { get; set; }

	[JsonPropertyName("system")]
	public string System { get; set; } = string.Empty;

	[JsonPropertyName("system_version")]
	public string? SystemVersion { get; set; }

	[JsonPropertyName("game_master")]
	public required string GameMaster { get; set; }

	[JsonPropertyName("start")]
	public DateTimeOffset Start { get; set; }

	[JsonPropertyName("duration")]
	public int DurationMinutes { get; set; }

	[JsonIgnore]
	public TimeSpan Duration => TimeSpan.FromMinutes(DurationMinutes);

	[JsonIgnore]
	public DateTimeOffset End => Start + Duration;

	[JsonPropertyName("player_max")]
	public int PlayerMax { get; set; }

	[JsonPropertyName("players_count")]
	public int? PlayerCount { get; set; }

	[JsonPropertyName("age_min")]
	public int? AgeMin { get; set; }

	[JsonPropertyName("tables")]
	public IList<Table> Tables { get; set; } = [];
}
