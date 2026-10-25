using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Conservices.Screen.Repositories.Serialization.Conservices;

public class ConservicesDatetimeConverter : JsonConverter<DateTime?>
{
	private const DateTimeStyles UtcStyles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;

	public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var content = reader.GetString();

		if (string.IsNullOrWhiteSpace(content))
			return null;

		if (DateTime.TryParse(content.Trim(), CultureInfo.InvariantCulture, UtcStyles, out var date))
			return date;
		
		Console.WriteLine($"Could not parse date: {content}");

		return null;
	}

	public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
}
