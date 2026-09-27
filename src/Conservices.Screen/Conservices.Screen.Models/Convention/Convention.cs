using System.Text.Json.Serialization;

namespace Conservices.Screen.Models.Convention;

public class Convention
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }
    
    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    [JsonPropertyName("begin_convention")]
    public DateTime? ConventionStart { get; set; }

    [JsonPropertyName("end_convention")]
    public DateTime? ConventionEnd { get; set; }

    [JsonPropertyName("begin_show")]
    public DateTime? ShowStart { get; set; }

    [JsonPropertyName("end_show")]
    public DateTime? ShowEnd { get; set; }

    [JsonPropertyName("description_overview")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("module_game")]
    public bool HasGameModule { get; set; }

    [JsonPropertyName("module_exhibitor")]
    public bool HasExhibitorModule { get; set; }

    [JsonPropertyName("module_programm")]
    public bool HasProgramModule { get; set; }
}