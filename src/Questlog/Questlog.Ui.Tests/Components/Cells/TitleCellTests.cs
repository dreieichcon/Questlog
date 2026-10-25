using Bunit;
using Questlog.Ui.Components.Cells;

namespace Questlog.Ui.Tests.Components.Cells;

[TestClass]
public class TitleCellTests : BunitContext
{
	private string RenderMarkup(int? minimumAge)
		=> Render<TitleCell>(parameters => parameters
			.Add(cell => cell.Title, "Drachenfels")
			.Add(cell => cell.MinimumAge, minimumAge)).Markup;

	[TestMethod]
	public void Render_ShowsTitle()
	{
		StringAssert.Contains(RenderMarkup(null), "Drachenfels");
	}

	[TestMethod]
	[DataRow(18, "age-18")]
	[DataRow(21, "age-18")]
	[DataRow(16, "age-16")]
	[DataRow(17, "age-16")]
	public void Render_MinimumAgeFromSixteen_ShowsBadgeOfMatchingClass(int age, string expectedClass)
	{
		StringAssert.Contains(RenderMarkup(age), expectedClass);
	}

	[TestMethod]
	[DataRow(null)]
	[DataRow(0)]
	[DataRow(12)]
	[DataRow(15)]
	public void Render_MinimumAgeBelowSixteen_ShowsNoBadge(int? age)
	{
		Assert.IsFalse(RenderMarkup(age).Contains("age-badge"));
	}
}
