using Bunit;
using Questlog.Ui.Components.Cells;

namespace Questlog.Ui.Tests.Components.Cells;

[TestClass]
public class StartTimeCellTests : FormatterContext
{
	private static readonly DateTimeOffset Start = Now.AddHours(3);

	private string RenderMarkup(bool showDay)
	{
		AddFormatter();
		return Render<StartTimeCell>(parameters => parameters
			.Add(cell => cell.Start, Start)
			.Add(cell => cell.ShowDay, showDay)).Markup;
	}

	[TestMethod]
	public void Render_WithoutDay_ShowsTimeOnly()
	{
		StringAssert.Contains(RenderMarkup(showDay: false), ">10:00<");
	}

	[TestMethod]
	public void Render_WithDay_ShowsWeekdayAndTime()
	{
		StringAssert.Contains(RenderMarkup(showDay: true), ">Mo. 10:00<");
	}

	[TestMethod]
	public void Render_ShowsTimeUntilStart()
	{
		StringAssert.Contains(RenderMarkup(showDay: false), "3 Stunden");
	}
}
