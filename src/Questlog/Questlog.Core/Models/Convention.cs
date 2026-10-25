using System.Text.Json.Serialization;

namespace Questlog.Core.Models;

public class Convention
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("name")]
	public required string Name { get; set; }

	[JsonPropertyName("logo")]
	public string? Logo { get; set; }

	[JsonPropertyName("begin_convention")]
	public DateTimeOffset? ConventionStart { get; set; }

	[JsonPropertyName("end_convention")]
	public DateTimeOffset? ConventionEnd { get; set; }

	[JsonPropertyName("module_game")]
	public bool HasGameModule { get; set; }

	[JsonPropertyName("module_exhibitor")]
	public bool HasExhibitorModule { get; set; }

	[JsonPropertyName("module_programm")]
	public bool HasProgramModule { get; set; }
}
