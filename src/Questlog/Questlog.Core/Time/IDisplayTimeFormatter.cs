namespace Questlog.Core.Time;

public interface IDisplayTimeFormatter
{
	public string Time(DateTimeOffset? value);

	public string TimeWithSeconds(DateTimeOffset? value);

	public string DateAndTime(DateTimeOffset? value);

	public string DayAndTime(DateTimeOffset? value);

	public string TimeDelta(DateTimeOffset? target);

	public IReadOnlyList<string> DurationParts(TimeSpan duration);
}
