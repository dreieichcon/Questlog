using Microsoft.Extensions.Time.Testing;
using Questlog.Core.Time;

namespace Questlog.Core.Tests.Time;

[TestClass]
public class DisplayTimeFormatterTests
{
	private static readonly DateTimeOffset Now = new(2030, 11, 14, 10, 0, 0, TimeSpan.Zero);

	private static readonly TimeZoneInfo Berlin = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");

	private static DisplayTimeFormatter CreateFormatter(TimeZoneInfo? timeZone = null)
	{
		var timeProvider = new FakeTimeProvider(Now);
		return new DisplayTimeFormatter(timeProvider, timeZone ?? Berlin);
	}

	[TestMethod]
	public void Time_SummerTimestamp_IsShownInBerlinSummerTime()
	{
		var formatter = CreateFormatter();

		var result = formatter.Time(new DateTimeOffset(2030, 7, 1, 10, 0, 0, TimeSpan.Zero));

		Assert.AreEqual("12:00", result);
	}

	[TestMethod]
	public void Time_WinterTimestamp_IsShownInBerlinWinterTime()
	{
		var formatter = CreateFormatter();

		var result = formatter.Time(new DateTimeOffset(2030, 1, 15, 10, 0, 0, TimeSpan.Zero));

		Assert.AreEqual("11:00", result);
	}

	[TestMethod]
	public void Time_ConfiguredUtcZone_IsShownUnchanged()
	{
		var formatter = CreateFormatter(TimeZoneInfo.Utc);

		var result = formatter.Time(new DateTimeOffset(2030, 7, 1, 10, 0, 0, TimeSpan.Zero));

		Assert.AreEqual("10:00", result);
	}

	[TestMethod]
	public void Time_TimestampWithOtherOffset_IsConvertedToConfiguredZone()
	{
		var formatter = CreateFormatter();

		var result = formatter.Time(new DateTimeOffset(2030, 1, 15, 5, 0, 0, TimeSpan.FromHours(-5)));

		Assert.AreEqual("11:00", result);
	}

	[TestMethod]
	public void Time_NullTimestamp_ReturnsEmptyString()
	{
		var formatter = CreateFormatter();

		Assert.AreEqual(string.Empty, formatter.Time(null));
	}

	[TestMethod]
	public void TimeWithSeconds_ReturnsLocalTimeWithSeconds()
	{
		var formatter = CreateFormatter();

		var result = formatter.TimeWithSeconds(new DateTimeOffset(2030, 7, 1, 10, 5, 9, TimeSpan.Zero));

		Assert.AreEqual("12:05:09", result);
	}

	[TestMethod]
	public void TimeWithSeconds_NullTimestamp_ReturnsEmptyString()
	{
		var formatter = CreateFormatter();

		Assert.AreEqual(string.Empty, formatter.TimeWithSeconds(null));
	}

	[TestMethod]
	public void DateAndTime_ReturnsIsoDateAndLocalTime()
	{
		var formatter = CreateFormatter();

		var result = formatter.DateAndTime(new DateTimeOffset(2030, 11, 14, 9, 5, 0, TimeSpan.Zero));

		Assert.AreEqual("2030-11-14 10:05", result);
	}

	[TestMethod]
	public void DateAndTime_NullTimestamp_ReturnsEmptyString()
	{
		var formatter = CreateFormatter();

		Assert.AreEqual(string.Empty, formatter.DateAndTime(null));
	}

	[TestMethod]
	[DataRow(2030, 11, 11, "Mo. 10:30")]
	[DataRow(2030, 11, 12, "Di. 10:30")]
	[DataRow(2030, 11, 13, "Mi. 10:30")]
	[DataRow(2030, 11, 14, "Do. 10:30")]
	[DataRow(2030, 11, 15, "Fr. 10:30")]
	[DataRow(2030, 11, 16, "Sa. 10:30")]
	[DataRow(2030, 11, 17, "So. 10:30")]
	public void DayAndTime_EveryWeekday_ReturnsGermanAbbreviationAndLocalTime(int year, int month, int day, string expected)
	{
		var formatter = CreateFormatter();

		var result = formatter.DayAndTime(new DateTimeOffset(year, month, day, 9, 30, 0, TimeSpan.Zero));

		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	public void DayAndTime_UtcTimeShortlyBeforeMidnight_UsesLocalWeekday()
	{
		var formatter = CreateFormatter();

		var result = formatter.DayAndTime(new DateTimeOffset(2030, 11, 16, 23, 30, 0, TimeSpan.Zero));

		Assert.AreEqual("So. 00:30", result);
	}

	[TestMethod]
	public void DayAndTime_NullTimestamp_ReturnsEmptyString()
	{
		var formatter = CreateFormatter();

		Assert.AreEqual(string.Empty, formatter.DayAndTime(null));
	}

	[TestMethod]
	[DataRow(-300, "Jetzt")]
	[DataRow(0, "Jetzt")]
	[DataRow(59, "Jetzt")]
	[DataRow(60, "1 Minute")]
	[DataRow(90, "1 Minute")]
	[DataRow(120, "2 Minuten")]
	[DataRow(3599, "59 Minuten")]
	public void TimeDelta_LessThanOneHour_ShowsMinutesOrJetzt(int secondsAhead, string expected)
	{
		var formatter = CreateFormatter();

		var result = formatter.TimeDelta(Now.AddSeconds(secondsAhead));

		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	[DataRow(3600, "1 Stunde")]
	[DataRow(3660, "1 Stunde 1 Minute")]
	[DataRow(7200, "2 Stunden")]
	[DataRow(9000, "2 Stunden 30 Minuten")]
	[DataRow(86340, "23 Stunden 59 Minuten")]
	public void TimeDelta_LessThanOneDay_ShowsHoursAndOnlyNonZeroMinutes(int secondsAhead, string expected)
	{
		var formatter = CreateFormatter();

		var result = formatter.TimeDelta(Now.AddSeconds(secondsAhead));

		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	[DataRow(86400, "1 Tag")]
	[DataRow(90000, "1 Tag 1 Stunde")]
	[DataRow(172800, "2 Tage")]
	[DataRow(183600, "2 Tage 3 Stunden")]
	[DataRow(183900, "2 Tage 3 Stunden")]
	public void TimeDelta_OneDayOrMore_ShowsDaysAndOnlyNonZeroHours(int secondsAhead, string expected)
	{
		var formatter = CreateFormatter();

		var result = formatter.TimeDelta(Now.AddSeconds(secondsAhead));

		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	public void TimeDelta_DoesNotDependOnConfiguredZone()
	{
		var target = Now.AddHours(5).AddMinutes(10);

		var berlin = CreateFormatter(Berlin).TimeDelta(target);
		var utc = CreateFormatter(TimeZoneInfo.Utc).TimeDelta(target);

		Assert.AreEqual(berlin, utc);
	}

	[TestMethod]
	public void TimeDelta_NullTimestamp_ReturnsEmptyString()
	{
		var formatter = CreateFormatter();

		Assert.AreEqual(string.Empty, formatter.TimeDelta(null));
	}

	[TestMethod]
	public void TimeDelta_FollowsTheClock()
	{
		var timeProvider = new FakeTimeProvider(Now);
		var formatter = new DisplayTimeFormatter(timeProvider, Berlin);
		var target = Now.AddMinutes(10);

		Assert.AreEqual("10 Minuten", formatter.TimeDelta(target));

		timeProvider.Advance(TimeSpan.FromMinutes(4));

		Assert.AreEqual("6 Minuten", formatter.TimeDelta(target));
	}
}
