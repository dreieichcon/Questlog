using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Ui.Pages.Viewers;

namespace Questlog.Ui.Tests.Pages.Viewers;

[TestClass]
public class ProgramViewTests : ViewerPageContext
{
	private IRenderedComponent<ProgramView> RenderView(string eventId, int itemCount)
	{
		var items = Enumerable.Range(1, itemCount)
			.Select(index => new ProgramItem { Id = $"{index}", Title = $"Punkt {index}", Start = Now.AddHours(index) })
			.ToList();
		var programService = Substitute.For<IProgramService>();
		programService.GetAllAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<IReadOnlyList<ProgramItem>>(items));
		Services.AddSingleton(programService);

		return Render<ProgramView>(parameters => parameters.Add(view => view.EventId, eventId));
	}

	[TestMethod]
	public void Render_ShowsConventionInHeaderAndViewNameInFooter()
	{
		var view = RenderView("c1", itemCount: 2);

		StringAssert.Contains(view.Find("header").TextContent, "Alpha Con");
		Assert.AreEqual("https://conservices.de/media/alpha.png", view.Find("header img").GetAttribute("src"));
		StringAssert.Contains(view.Find("footer").TextContent, "Programm");
	}

	[TestMethod]
	public void Render_ShowsProgramRows()
	{
		var view = RenderView("c1", itemCount: 2);

		Assert.AreEqual(2, view.FindAll("tbody tr").Count);
	}

	[TestMethod]
	public void Render_FewItems_ShowsShrunkenCycleInFooter()
	{
		var view = RenderView("c1", itemCount: 3);

		view.WaitForAssertion(() => StringAssert.Contains(view.Find("footer").TextContent, "1 / 3"));
	}

	[TestMethod]
	public void Render_ManyItems_ShowsFullCycleInFooter()
	{
		var view = RenderView("c1", itemCount: 25);

		view.WaitForAssertion(() => StringAssert.Contains(view.Find("footer").TextContent, "1 / 20"));
	}

	[TestMethod]
	public void Render_UnknownEvent_ShowsNotFoundTextWithoutFrame()
	{
		var view = RenderView("missing", itemCount: 2);

		StringAssert.Contains(view.Markup, "Die Veranstaltung missing existiert nicht");
		Assert.AreEqual(0, view.FindAll("header").Count);
		Assert.AreEqual(0, view.FindAll("table").Count);
	}
}
