using System.Text.RegularExpressions;
using Bunit;
using Questlog.Ui.Components.Cells;

namespace Questlog.Ui.Tests.Components.Cells;

[TestClass]
public class BreakableTextTests : BunitContext
{
	private string RenderMarkup(string text)
		=> Render<BreakableText>(parameters => parameters.Add(component => component.Text, text)).Markup;

	[TestMethod]
	public void Render_NameWithCapitals_InsertsBreakBetweenParts()
	{
		var markup = RenderMarkup("AnnaLenaMarie");

		Assert.AreEqual(2, Regex.Count(markup, "<wbr"));
		StringAssert.Contains(markup, "Anna");
		StringAssert.Contains(markup, "Marie");
	}

	[TestMethod]
	public void Render_NameWithoutBreakPoint_InsertsNoBreak()
	{
		Assert.AreEqual(0, Regex.Count(RenderMarkup("Anna"), "<wbr"));
	}

	[TestMethod]
	public void Render_MarkupInText_IsEncoded()
	{
		var markup = RenderMarkup("<script>x");

		Assert.IsFalse(markup.Contains("<script>"));
		StringAssert.Contains(markup, "&lt;script&gt;");
	}
}
