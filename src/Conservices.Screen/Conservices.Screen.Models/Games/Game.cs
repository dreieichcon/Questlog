using System.Text.Json.Serialization;
using Conservices.Screen.Models.Interfaces;
using Conservices.Screen.Models.Misc;

namespace Conservices.Screen.Models.Games;

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class Game : IHasStartTime
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("title")]
	public required string Title { get; set; }

	[JsonPropertyName("system")]
	public string System { get; set; } = string.Empty;

	[JsonPropertyName("system_version")]
	public string? SystemVersion { get; set; } = string.Empty;

	[JsonPropertyName("chars")]
	public string Characters { get; set; } = string.Empty;

	[JsonPropertyName("content_note")]
	public string? ContentNote { get; set; }

	[JsonPropertyName("teaser")]
	public string Teaser { get; set; } = string.Empty;

	[JsonPropertyName("game_master")]
	public required string GameMaster { get; set; }

	[JsonPropertyName("start")]
	public DateTime Start { get; set; }

	[JsonPropertyName("duration")]
	public int DurationMinutes { get; set; }

	[JsonIgnore]
	public TimeSpan Duration => TimeSpan.FromMinutes(DurationMinutes);

	public DateTime? End => Start + Duration;

	[JsonPropertyName("player_min")]
	public int PlayerMin { get; set; }

	[JsonPropertyName("player_max")]
	public int PlayerMax { get; set; }
	
	[JsonPropertyName("player_count")]
	public int? PlayerCount { get; set; }

	[JsonPropertyName("age_min")]
	public int? AgeMin { get; set; }

	[JsonPropertyName("age_max")]
	public int? AgeMax { get; set; }

	[JsonPropertyName("has_free_slots")]
	public bool HasFreeSlots { get; set; }

	[JsonPropertyName("language")]
	public required Language Language { get; set; }

	[JsonPropertyName("tags")]
	public IList<Label> Tags { get; set; } = [];

	[JsonPropertyName("tables")]
	public IList<Table> Tables { get; set; } = [];
}

public class Language
{
	[JsonPropertyName("name")]
	public required string Name { get; set; }
}