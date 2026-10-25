using System.Text.Json.Serialization;

namespace Questlog.Core.Models;

public class ProgramItem : IHasStartTime
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("title")]
	public string Title { get; set; } = string.Empty;

	[JsonPropertyName("start")]
	public DateTimeOffset Start { get; set; }

	[JsonPropertyName("duration")]
	public int DurationMinutes { get; set; }

	[JsonIgnore]
	public TimeSpan Duration => TimeSpan.FromMinutes(DurationMinutes);

	[JsonIgnore]
	public DateTimeOffset End => Start + Duration;

	[JsonPropertyName("age_min")]
	public int? AgeMin { get; set; }

	[JsonPropertyName("tags")]
	public IList<Tag> Tags { get; set; } = [];

	[JsonPropertyName("tables")]
	public IList<Table> Tables { get; set; } = [];

	[JsonIgnore]
	public Table? Table => Tables.FirstOrDefault();

	[JsonIgnore]
	public Tag? Tag => Tags.FirstOrDefault();
}
