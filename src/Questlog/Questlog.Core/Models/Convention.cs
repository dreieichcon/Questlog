using System.Text.Json.Serialization;
using Questlog.Core.Configuration;

namespace Questlog.Core.Models;

public class Convention
{
	[JsonPropertyName("id")]
	public required string Id { get; set; }

	[JsonPropertyName("name")]
	public required string Name { get; set; }

	[JsonPropertyName("logo")]
	public string? Logo { get; set; }

	[JsonIgnore]
	public string? LogoUrl => Logo switch
	{
		null or "" => null,
		var logo when logo.StartsWith("http", StringComparison.OrdinalIgnoreCase) => logo,
		var logo => ConservicesSite.BaseUrl + logo,
	};

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
