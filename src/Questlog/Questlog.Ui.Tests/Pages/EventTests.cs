using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Core.Time;
using Questlog.Ui.Pages;

namespace Questlog.Ui.Tests.Pages;

[TestClass]
public class EventTests : BunitContext
{
	private readonly IConventionService _conventionService = Substitute.For<IConventionService>();

	private static Convention CreateConvention(bool program, bool games)
		=> new()
		{
			Id = "c1",
			Name = "Alpha Con",
			HasProgramModule = program,
			HasGameModule = games,
			ConventionStart = new DateTimeOffset(2030, 7, 5, 10, 0, 0, TimeSpan.Zero),
			ConventionEnd = new DateTimeOffset(2030, 7, 7, 18, 0, 0, TimeSpan.Zero),
		};

	private IRenderedComponent<Event> RenderEvent(Convention? convention)
	{
		var timeProvider = new FakeTimeProvider();
		_conventionService.GetAsync("c1", Arg.Any<CancellationToken>()).Returns(Task.FromResult(convention));
		Services.AddSingleton(_conventionService);
		Services.AddSingleton<IDisplayTimeFormatter>(new DisplayTimeFormatter(timeProvider, TimeZoneInfo.Utc));

		return Render<Event>(parameters => parameters.Add(page => page.EventId, "c1"));
	}

	private static string[] Hrefs(IRenderedComponent<Event> page)
		=> [.. page.FindAll("a.menu-link").Select(link => link.GetAttribute("href")!)];

	[TestMethod]
	public void Render_UnknownEvent_ShowsNotFoundText()
	{
		var page = RenderEvent(null);

		StringAssert.Contains(page.Markup, "Die Veranstaltung c1 existiert nicht");
		Assert.AreEqual(0, page.FindAll("a.menu-link").Count);
	}

	[TestMethod]
	public void Render_ShowsNameAndDates()
	{
		var page = RenderEvent(CreateConvention(program: true, games: true));

		StringAssert.Contains(page.Markup, "Alpha Con");
		StringAssert.Contains(page.Markup, "2030-07-05 10:00");
		StringAssert.Contains(page.Markup, "2030-07-07 18:00");
	}

	[TestMethod]
	public void Render_ProgramModuleOnly_ShowsProgramViews()
	{
		var page = RenderEvent(CreateConvention(program: true, games: false));

		CollectionAssert.AreEqual(new[] { "event/c1/program", "event/c1/program/rooms" }, Hrefs(page));
	}

	[TestMethod]
	public void Render_GameModuleOnly_ShowsGameViews()
	{
		var page = RenderEvent(CreateConvention(program: false, games: true));

		CollectionAssert.AreEqual(new[] { "event/c1/games", "event/c1/gamesPerBuilding", "event/c1/gamesBySystem" }, Hrefs(page));
	}

	[TestMethod]
	public void Render_BothModules_ShowsAllViewsIncludingCombined()
	{
		var page = RenderEvent(CreateConvention(program: true, games: true));

		CollectionAssert.AreEqual(
			new[]
			{
				"event/c1/program",
				"event/c1/program/rooms",
				"event/c1/games",
				"event/c1/gamesPerBuilding",
				"event/c1/gamesBySystem",
				"event/c1/programAndGames",
			},
			Hrefs(page));
	}

	[TestMethod]
	public void Render_NoModules_ShowsNoViews()
	{
		var page = RenderEvent(CreateConvention(program: false, games: false));

		Assert.AreEqual(0, page.FindAll("a.menu-link").Count);
	}
}
