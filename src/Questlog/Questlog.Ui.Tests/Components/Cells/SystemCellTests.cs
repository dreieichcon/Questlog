using Bunit;
using Questlog.Ui.Components.Cells;

namespace Questlog.Ui.Tests.Components.Cells;

[TestClass]
public class SystemCellTests : BunitContext
{
	private string RenderMarkup(string name, string? version)
		=> Render<SystemCell>(parameters => parameters
			.Add(cell => cell.Name, name)
			.Add(cell => cell.Version, version)).Markup;

	[TestMethod]
	public void Render_VersionNotInName_ShowsNameAndVersion()
	{
		var markup = RenderMarkup("D&D", "5e");

		StringAssert.Contains(markup, "D&amp;D");
		StringAssert.Contains(markup, "5e");
	}

	[TestMethod]
	public void Render_VersionAlreadyInName_ShowsVersionOnce()
	{
		var markup = RenderMarkup("D&D 5e", "5e");

		Assert.AreEqual(1, markup.Split("5e").Length - 1);
	}

	[TestMethod]
	[DataRow(null)]
	[DataRow("")]
	public void Render_NoVersion_ShowsOnlyName(string? version)
	{
		var markup = RenderMarkup("Pathfinder", version);

		Assert.IsFalse(markup.Contains("<wbr"));
	}

	[TestMethod]
	public void Render_MarkupInName_IsEncoded()
	{
		var markup = RenderMarkup("<b>Fett</b>", null);

		Assert.IsFalse(markup.Contains("<b>"));
		StringAssert.Contains(markup, "&lt;b&gt;");
	}
}
