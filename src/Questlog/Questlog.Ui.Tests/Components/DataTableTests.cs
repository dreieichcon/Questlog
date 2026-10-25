using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Ui.Components;

namespace Questlog.Ui.Tests.Components;

[TestClass]
public class DataTableTests : BunitContext
{
	private readonly IDisplaySyncService _displaySync = Substitute.For<IDisplaySyncService>();

	private IRenderedComponent<DataTable<string>> RenderTable(IReadOnlyList<string> items, int displayIndex = 0)
	{
		_displaySync.MaxItems.Returns(20);
		_displaySync.DisplayIndex.Returns(displayIndex);
		Services.AddSingleton(_displaySync);
		JSInterop.Mode = JSRuntimeMode.Loose;

		return Render<DataTable<string>>(parameters => parameters
			.Add(table => table.Items, items)
			.Add(table => table.Header, "<tr><th>Kopf</th></tr>")
			.Add(table => table.RowTemplate, item => $"<td>{item}</td>")
			.Add(table => table.EmptyText, "Nichts mehr"));
	}

	private static List<string> CreateItems(int count)
		=> [.. Enumerable.Range(0, count).Select(index => $"Eintrag {index}")];

	private static string ActiveRowId(IRenderedComponent<DataTable<string>> table)
		=> table.Find(".row-active").Id!;

	[TestMethod]
	public void Render_ShowsHeaderAndOneRowPerItem()
	{
		var table = RenderTable(CreateItems(3));

		Assert.AreEqual("Kopf", table.Find("thead").TextContent);
		Assert.AreEqual(3, table.FindAll("tbody tr").Count);
		Assert.AreEqual("Eintrag 2", table.Find("#row-2").TextContent);
	}

	[TestMethod]
	public void Render_MoreItemsThanMaxItems_ShowsAllItems()
	{
		var table = RenderTable(CreateItems(25));

		Assert.AreEqual(25, table.FindAll("tbody tr").Count);
	}

	[TestMethod]
	public void Render_EnoughItems_MarksRowOfDisplayIndexAsActive()
	{
		var table = RenderTable(CreateItems(20), displayIndex: 7);

		Assert.AreEqual("row-7", ActiveRowId(table));
	}

	[TestMethod]
	public void Render_FewerItemsThanMaxItems_CyclesActiveRow()
	{
		var table = RenderTable(CreateItems(3), displayIndex: 7);

		Assert.AreEqual("row-1", ActiveRowId(table));
	}

	[TestMethod]
	public void Render_NoItems_ShowsEmptyTextAndNoRows()
	{
		var table = RenderTable([]);

		Assert.AreEqual(0, table.FindAll("tbody tr").Count);
		Assert.AreEqual("Nichts mehr", table.Find(".empty-text").TextContent);
	}

	[TestMethod]
	public void Render_WithItems_ShowsNoEmptyText()
	{
		var table = RenderTable(CreateItems(1));

		Assert.AreEqual(0, table.FindAll(".empty-text").Count);
	}

	[TestMethod]
	public void DisplayChanged_MovesActiveRowAndScrollsToIt()
	{
		var table = RenderTable(CreateItems(5), displayIndex: 0);
		_displaySync.DisplayIndex.Returns(3);

		_displaySync.DisplayChanged += Raise.Event();

		table.WaitForAssertion(() => Assert.AreEqual("row-3", ActiveRowId(table)));
		var invocation = JSInterop.VerifyInvoke("scrollToElement");
		Assert.AreEqual("row-3", invocation.Arguments[0]);
	}

	[TestMethod]
	public async Task Dispose_RemovesDisplayChangedSubscription()
	{
		RenderTable(CreateItems(5));
		await DisposeComponentsAsync();

		_displaySync.DisplayChanged += Raise.Event();

		Assert.AreEqual(0, JSInterop.Invocations.Count);
	}
}
