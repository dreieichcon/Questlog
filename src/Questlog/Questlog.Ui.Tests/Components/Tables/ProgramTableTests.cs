using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Ui.Components.Tables;

namespace Questlog.Ui.Tests.Components.Tables;

[TestClass]
public class ProgramTableTests : TableContext
{
	private readonly IProgramService _programService = Substitute.For<IProgramService>();

	private static ProgramItem CreateItem(string title, DateTimeOffset start)
		=> new()
		{
			Id = title,
			Title = title,
			Start = start,
			Tags = [new Tag { Name = "Vortrag" }],
			Tables = [new Table { Name = "Saal 1" }],
		};

	private IRenderedComponent<ProgramTable> RenderTable(params ProgramItem[] items)
	{
		_programService.GetAllAsync("con1", Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<IReadOnlyList<ProgramItem>>(items));
		Services.AddSingleton(_programService);

		return Render<ProgramTable>(parameters => parameters.Add(table => table.EventId, "con1"));
	}

	[TestMethod]
	public void Render_ShowsTitleTypeLocationAndStart()
	{
		var table = RenderTable(CreateItem("Eroeffnung", Now.AddHours(1)));

		var row = table.Find("tbody tr").TextContent;
		StringAssert.Contains(row, "Eroeffnung");
		StringAssert.Contains(row, "Vortrag");
		StringAssert.Contains(row, "Saal 1");
		StringAssert.Contains(row, "08:00");
	}

	[TestMethod]
	public void Render_ItemsAlreadyStarted_AreHidden()
	{
		var table = RenderTable(CreateItem("Alt", Now.AddMinutes(-5)));

		Assert.AreEqual(0, table.FindAll("tbody tr").Count);
		StringAssert.Contains(table.Find(".empty-text").TextContent, "Programmpunkte");
	}
}
