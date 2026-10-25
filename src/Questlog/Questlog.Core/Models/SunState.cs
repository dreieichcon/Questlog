using System.Text.Json.Serialization;

namespace Questlog.Core.Models;

public class SunState
{
	[JsonPropertyName("date")]
	public DateOnly Date { get; set; }

	[JsonPropertyName("sunrise")]
	public TimeOnly Sunrise { get; set; }

	[JsonPropertyName("sunset")]
	public TimeOnly Sunset { get; set; }
}
