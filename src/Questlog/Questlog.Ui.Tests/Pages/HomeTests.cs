using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Ui.Pages;

namespace Questlog.Ui.Tests.Pages;

[TestClass]
public class HomeTests : BunitContext
{
	private readonly IConventionService _conventionService = Substitute.For<IConventionService>();

	private IRenderedComponent<Home> RenderHome(params Convention[] conventions)
	{
		_conventionService.GetAllAsync(Arg.Any<CancellationToken>())
			.Returns(Task.FromResult<IReadOnlyList<Convention>>(conventions));
		Services.AddSingleton(_conventionService);

		return Render<Home>();
	}

	[TestMethod]
	public void Render_ShowsLinkPerConvention()
	{
		var home = RenderHome(
			new Convention { Id = "a1", Name = "Alpha Con" },
			new Convention { Id = "b2", Name = "Beta Con" });

		var links = home.FindAll("a.menu-link");

		Assert.AreEqual(2, links.Count);
		Assert.AreEqual("Alpha Con", links[0].TextContent);
		Assert.AreEqual("event/a1", links[0].GetAttribute("href"));
		Assert.AreEqual("event/b2", links[1].GetAttribute("href"));
	}

	[TestMethod]
	public void Render_ShowsSelectionHint()
	{
		var home = RenderHome(new Convention { Id = "a1", Name = "Alpha Con" });

		StringAssert.Contains(home.Markup, "Veranstaltung auswählen");
	}

	[TestMethod]
	public void Render_NoConventions_ShowsEmptyText()
	{
		var home = RenderHome();

		Assert.AreEqual(0, home.FindAll("a.menu-link").Count);
		StringAssert.Contains(home.Markup, "Keine Veranstaltungen verfügbar");
	}
}
