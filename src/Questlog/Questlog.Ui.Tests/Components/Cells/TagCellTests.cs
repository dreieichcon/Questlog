using Bunit;
using Questlog.Core.Models;
using Questlog.Ui.Components.Cells;

namespace Questlog.Ui.Tests.Components.Cells;

[TestClass]
public class TagCellTests : BunitContext
{
	private string RenderMarkup(params Tag[] tags)
		=> Render<TagCell>(parameters => parameters.Add(cell => cell.Tags, tags)).Markup;

	[TestMethod]
	public void Render_MultipleTags_ShowsOnlyFirstName()
	{
		var markup = RenderMarkup(new Tag { Name = "Vortrag" }, new Tag { Name = "Workshop" });

		StringAssert.Contains(markup, "Vortrag");
		Assert.IsFalse(markup.Contains("Workshop"));
	}

	[TestMethod]
	public void Render_NoTags_ShowsNoText()
	{
		StringAssert.Contains(RenderMarkup(), "></td>");
	}
}
