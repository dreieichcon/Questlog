using Bunit;
using Questlog.Ui.Components.Cells;

namespace Questlog.Ui.Tests.Components.Cells;

[TestClass]
public class DurationCellTests : FormatterContext
{
	private string RenderMarkup(TimeSpan duration)
	{
		AddFormatter();
		return Render<DurationCell>(parameters => parameters.Add(cell => cell.Duration, duration)).Markup;
	}

	[TestMethod]
	public void Render_HoursAndMinutes_ShowsEachPartInItsOwnLine()
	{
		var markup = RenderMarkup(TimeSpan.FromMinutes(150));

		StringAssert.Contains(markup, ">2 Std.<");
		StringAssert.Contains(markup, ">30 Min.<");
	}

	[TestMethod]
	public void Render_OnlyMinutes_ShowsSingleLine()
	{
		var markup = RenderMarkup(TimeSpan.FromMinutes(45));

		StringAssert.Contains(markup, ">45 Min.<");
		Assert.IsFalse(markup.Contains("Std."));
	}
}
