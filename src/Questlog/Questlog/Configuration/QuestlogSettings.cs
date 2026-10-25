using System.Globalization;
using Microsoft.Extensions.Configuration;
using Questlog.Core.Configuration;

namespace Questlog.Configuration;

public record QuestlogSettings(TimeZoneInfo TimeZone, GeoLocation Location)
{
	private const string TimeZoneKey = "QUESTLOG_TZ";

	private const string LatitudeKey = "QUESTLOG_LAT";

	private const string LongitudeKey = "QUESTLOG_LNG";

	private const string DefaultTimeZoneId = "Europe/Berlin";

	private static readonly GeoLocation DefaultLocation = new(50.02485985471239, 8.66270997993345);

	/// <summary>
	/// Reads the settings and falls back to the defaults for missing or invalid values.
	/// </summary>
	public static QuestlogSettings FromConfiguration(IConfiguration configuration)
		=> new(ReadTimeZone(configuration[TimeZoneKey]), ReadLocation(configuration[LatitudeKey], configuration[LongitudeKey]));

	private static TimeZoneInfo ReadTimeZone(string? id)
		=> FindTimeZone(id) ?? FindTimeZone(DefaultTimeZoneId) ?? TimeZoneInfo.Utc;

	private static TimeZoneInfo? FindTimeZone(string? id)
	{
		if (string.IsNullOrWhiteSpace(id))
			return null;

		return TimeZoneInfo.TryFindSystemTimeZoneById(id.Trim(), out var timeZone) ? timeZone : null;
	}

	private static GeoLocation ReadLocation(string? latitude, string? longitude)
	{
		var hasLatitude = TryParseInRange(latitude, 90, out var parsedLatitude);
		var hasLongitude = TryParseInRange(longitude, 180, out var parsedLongitude);

		return hasLatitude && hasLongitude ? new GeoLocation(parsedLatitude, parsedLongitude) : DefaultLocation;
	}

	private static bool TryParseInRange(string? text, double limit, out double value)
	{
		var isParsed = double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
		return isParsed && Math.Abs(value) <= limit;
	}
}
