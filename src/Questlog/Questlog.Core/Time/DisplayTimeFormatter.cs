using System.Globalization;
using Questlog.Core.Resources;

namespace Questlog.Core.Time;

public class DisplayTimeFormatter(TimeProvider timeProvider, TimeZoneInfo timeZone) : IDisplayTimeFormatter
{
	private const string TimeFormat = "HH:mm";

	private const string TimeWithSecondsFormat = "HH:mm:ss";

	private const string DateAndTimeFormat = "yyyy-MM-dd HH:mm";

	/// <summary>
	/// Formats the time of day in the configured time zone.
	/// </summary>
	public string Time(DateTimeOffset? value)
		=> value.HasValue ? ToLocal(value.Value).ToString(TimeFormat, CultureInfo.InvariantCulture) : string.Empty;

	/// <summary>
	/// Formats the time of day including seconds in the configured time zone.
	/// </summary>
	public string TimeWithSeconds(DateTimeOffset? value)
		=> value.HasValue ? ToLocal(value.Value).ToString(TimeWithSecondsFormat, CultureInfo.InvariantCulture) : string.Empty;

	/// <summary>
	/// Formats date and time of day in the configured time zone.
	/// </summary>
	public string DateAndTime(DateTimeOffset? value)
		=> value.HasValue ? ToLocal(value.Value).ToString(DateAndTimeFormat, CultureInfo.InvariantCulture) : string.Empty;

	/// <summary>
	/// Formats weekday abbreviation and time of day in the configured time zone.
	/// </summary>
	public string DayAndTime(DateTimeOffset? value)
	{
		if (!value.HasValue)
			return string.Empty;

		var local = ToLocal(value.Value);
		var weekday = Strings.Get($"Weekday_{local.DayOfWeek}");

		return $"{weekday} {local.ToString(TimeFormat, CultureInfo.InvariantCulture)}";
	}

	/// <summary>
	/// Formats the remaining time until the target, omitting zero parts.
	/// </summary>
	public string TimeDelta(DateTimeOffset? target)
	{
		if (!target.HasValue)
			return string.Empty;

		var delta = target.Value - timeProvider.GetUtcNow();

		if (delta < TimeSpan.FromMinutes(1))
			return Strings.Get("Delta_Now");

		if (delta.TotalDays >= 1)
			return Join(Days(delta.Days), Hours(delta.Hours));

		if (delta.TotalHours >= 1)
			return Join(Hours(delta.Hours), Minutes(delta.Minutes));

		return Minutes(delta.Minutes);
	}

	private DateTimeOffset ToLocal(DateTimeOffset value)
		=> TimeZoneInfo.ConvertTime(value, timeZone);

	private static string Days(int count) => Unit(count, "Delta_Day");

	private static string Hours(int count) => Unit(count, "Delta_Hour");

	private static string Minutes(int count) => Unit(count, "Delta_Minute");

	private static string Unit(int count, string keyPrefix)
		=> count switch
		{
			<= 0 => string.Empty,
			1 => Strings.Format($"{keyPrefix}_One", count),
			_ => Strings.Format($"{keyPrefix}_Many", count),
		};

	private static string Join(string first, string second)
		=> string.IsNullOrEmpty(second) ? first : $"{first} {second}";
}
