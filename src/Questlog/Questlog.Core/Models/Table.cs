using System.Text.Json.Serialization;

namespace Questlog.Core.Models;

public class Table
{
	[JsonPropertyName("name")]
	public required string Name { get; set; }

	[JsonPropertyName("wheelchair_accessible")]
	public bool Accessible { get; set; }

	[JsonPropertyName("location")]
	public Location? Location { get; set; }
}
