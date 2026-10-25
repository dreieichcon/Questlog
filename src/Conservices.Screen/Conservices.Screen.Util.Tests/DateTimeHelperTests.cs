namespace Conservices.Screen.Util.Tests;

[TestClass]
public class DateTimeHelperTests
{
	[TestMethod]
	public void ToLocal_SummerUtcTime_AddsTwoHours()
	{
		var utc = new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc);

		var local = utc.ToLocal();

		Assert.AreEqual(new DateTime(2026, 7, 1, 12, 0, 0), local);
	}

	[TestMethod]
	public void ToLocal_WinterUtcTime_AddsOneHour()
	{
		var utc = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc);

		var local = utc.ToLocal();

		Assert.AreEqual(new DateTime(2026, 1, 15, 11, 0, 0), local);
	}

	[TestMethod]
	public void DateAndTime_NullDate_ReturnsEmptyString()
	{
		DateTime? date = null;

		Assert.AreEqual(string.Empty, date.DateAndTime());
	}

	[TestMethod]
	public void DateAndTime_ValidDate_ReturnsIsoDateAndTime()
	{
		DateTime? date = new DateTime(2026, 10, 25, 14, 5, 0);

		Assert.AreEqual("2026-10-25 14:05", date.DateAndTime());
	}

	[TestMethod]
	public void DayAndTime_NullDate_ReturnsEmptyString()
	{
		DateTime? date = null;

		Assert.AreEqual(string.Empty, date.DayAndTime());
	}

	[TestMethod]
	[DataRow(2026, 10, 25, "So. 14:30")]
	[DataRow(2026, 10, 26, "Mo. 14:30")]
	[DataRow(2026, 10, 27, "Di. 14:30")]
	[DataRow(2026, 10, 28, "Mi. 14:30")]
	[DataRow(2026, 10, 29, "Do. 14:30")]
	[DataRow(2026, 10, 30, "Fr. 14:30")]
	[DataRow(2026, 10, 31, "Sa. 14:30")]
	public void DayAndTime_EveryWeekday_ReturnsGermanAbbreviation(int year, int month, int day, string expected)
	{
		var date = new DateTime(year, month, day, 14, 30, 0);

		Assert.AreEqual(expected, date.DayAndTime());
	}

	[TestMethod]
	public void DayAndTime_NullableWithValue_MatchesNonNullableResult()
	{
		var date = new DateTime(2026, 10, 25, 14, 30, 0);
		DateTime? nullableDate = date;

		Assert.AreEqual(date.DayAndTime(), nullableDate.DayAndTime());
	}

	[TestMethod]
	public void TimeDelta_NullDate_ReturnsEmptyString()
	{
		DateTime? date = null;

		Assert.AreEqual(string.Empty, date.TimeDelta());
	}

	[TestMethod]
	public void TimeDelta_StartInThePast_ReturnsJetzt()
	{
		var date = DateTime.UtcNow.AddMinutes(-5);

		Assert.AreEqual("Jetzt", date.TimeDelta());
	}

	[TestMethod]
	public void TimeDelta_StartInSecondsOnly_ReturnsJetzt()
	{
		var date = DateTime.UtcNow.AddSeconds(20);

		Assert.AreEqual("Jetzt", date.TimeDelta());
	}

	[TestMethod]
	public void TimeDelta_OneMinuteAhead_ReturnsSingularMinute()
	{
		var date = DateTime.UtcNow.AddSeconds(90);

		Assert.AreEqual("1 Minute", date.TimeDelta());
	}

	[TestMethod]
	public void TimeDelta_LessThanOneHour_ReturnsMinutes()
	{
		var date = DateTime.UtcNow.AddMinutes(30).AddSeconds(30);

		Assert.AreEqual("30 Minuten", date.TimeDelta());
	}

	[TestMethod]
	public void TimeDelta_HoursAndMinutes_ReturnsBoth()
	{
		var date = DateTime.UtcNow.AddHours(2).AddMinutes(30).AddSeconds(30);

		Assert.AreEqual("2 Stunden 30 Minuten", date.TimeDelta());
	}

	[TestMethod]
	public void TimeDelta_OneHourAndOneMinute_ReturnsSingularUnits()
	{
		var date = DateTime.UtcNow.AddHours(1).AddMinutes(1).AddSeconds(30);

		Assert.AreEqual("1 Stunde 1 Minute", date.TimeDelta());
	}

	[TestMethod]
	public void TimeDelta_DaysAndHours_ReturnsBoth()
	{
		var date = DateTime.UtcNow.AddDays(2).AddHours(3).AddMinutes(30);

		Assert.AreEqual("2 Tage 3 Stunden", date.TimeDelta());
	}

	[TestMethod]
	public void TimeDelta_OneDayAndOneHour_ReturnsSingularUnits()
	{
		var date = DateTime.UtcNow.AddDays(1).AddHours(1).AddMinutes(30);

		Assert.AreEqual("1 Tag 1 Stunde", date.TimeDelta());
	}

	[TestMethod]
	public void TimeDelta_NullableWithValue_MatchesNonNullableResult()
	{
		var date = DateTime.UtcNow.AddHours(5).AddMinutes(30);
		DateTime? nullableDate = date;

		Assert.AreEqual(date.TimeDelta(), nullableDate.TimeDelta());
	}
}
