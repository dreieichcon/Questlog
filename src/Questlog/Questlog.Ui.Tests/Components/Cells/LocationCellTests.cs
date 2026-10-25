using Bunit;
using Questlog.Core.Models;
using Questlog.Ui.Components.Cells;

namespace Questlog.Ui.Tests.Components.Cells;

[TestClass]
public class LocationCellTests : BunitContext
{
	private static Table CreateTable(string name, bool accessible = false)
		=> new() { Name = name, Accessible = accessible };

	private string RenderMarkup(params Table[] tables)
		=> Render<LocationCell>(parameters => parameters.Add(cell => cell.Tables, tables)).Markup;

	[TestMethod]
	public void Render_MultipleTables_ShowsOnlyFirstName()
	{
		var markup = RenderMarkup(CreateTable("Tisch 1"), CreateTable("Tisch 2"));

		StringAssert.Contains(markup, "Tisch 1");
		Assert.IsFalse(markup.Contains("Tisch 2"));
	}

	[TestMethod]
	public void Render_AnyTableAccessible_ShowsWheelchair()
	{
		var markup = RenderMarkup(CreateTable("Tisch 1"), CreateTable("Tisch 2", accessible: true));

		StringAssert.Contains(markup, "wheelchair");
	}

	[TestMethod]
	public void Render_NoTableAccessible_ShowsNoWheelchair()
	{
		Assert.IsFalse(RenderMarkup(CreateTable("Tisch 1")).Contains("wheelchair"));
	}

	[TestMethod]
	public void Render_NoTables_ShowsNoWheelchair()
	{
		Assert.IsFalse(RenderMarkup().Contains("wheelchair"));
	}
}
