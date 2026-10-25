using System.Text.Json.Serialization;

namespace Questlog.Core.Models;

public class Tag
{
	[JsonPropertyName("name")]
	public required string Name { get; set; }
}
