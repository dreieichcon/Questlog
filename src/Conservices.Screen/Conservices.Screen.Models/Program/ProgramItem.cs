using System.Text.Json.Serialization;
using Conservices.Screen.Models.Games;
using Conservices.Screen.Models.Interfaces;
using Conservices.Screen.Models.Misc;

namespace Conservices.Screen.Models.Program;

public class ProgramItem : IHasStartTime
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("title")]
	public string Title { get; set; } = string.Empty;

	[JsonPropertyName("teaser")]
	public string Teaser { get; set; } = string.Empty;

	[JsonPropertyName("content_note")]
	public string? ContentNote { get; set; }

	[JsonPropertyName("host")]
	public required string Host { get; set; }

	[JsonPropertyName("start")]
	public DateTime Start { get; set; }

	[JsonIgnore]
	public DateTime End => Start.Add(Duration);

	[JsonPropertyName("duration")]
	public int DurationMinutes { get; set; }

	[JsonIgnore]
	public TimeSpan Duration => TimeSpan.FromMinutes(DurationMinutes);

	[JsonPropertyName("age_min")]
	public int? MinimumAge { get; set; }

	[JsonPropertyName("age_max")]
	public int? MaximumAge { get; set; }

	[JsonPropertyName("language")]
	public required Language Language { get; set; }

	[JsonPropertyName("tags")]
	public IList<Label> Labels { get; set; } = [];

	[JsonPropertyName("tables")]
	public IList<Table> Tables { get; set; } = [];

	public Table? Table => Tables.FirstOrDefault();

	public Label? Label => Labels.FirstOrDefault();
}