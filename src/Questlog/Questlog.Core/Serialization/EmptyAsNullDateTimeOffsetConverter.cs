using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Questlog.Core.Serialization;

public class EmptyAsNullDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
	/// <summary>
	/// Reads an ISO-8601 timestamp, treating empty and unparsable values as null.
	/// </summary>
	public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var content = reader.GetString();

		if (string.IsNullOrWhiteSpace(content))
			return null;

		var isParsed = DateTimeOffset.TryParse(content.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date);

		return isParsed ? date : null;
	}

	/// <summary>
	/// Writes the timestamp in round-trip format.
	/// </summary>
	public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
	{
		if (value.HasValue)
			writer.WriteStringValue(value.Value);
		else
			writer.WriteNullValue();
	}
}
