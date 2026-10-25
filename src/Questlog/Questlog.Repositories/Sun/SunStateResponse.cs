using System.Text.Json.Serialization;
using Questlog.Core.Models;

namespace Questlog.Repositories.Sun;

public class SunStateResponse
{
	[JsonPropertyName("status")]
	public string? Status { get; set; }

	[JsonPropertyName("results")]
	public SunState? Results { get; set; }
}
