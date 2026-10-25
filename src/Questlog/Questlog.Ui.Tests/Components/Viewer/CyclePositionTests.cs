using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Ui.Components.Viewer;

namespace Questlog.Ui.Tests.Components.Viewer;

[TestClass]
public class CyclePositionTests : BunitContext
{
	private readonly IDisplaySyncService _displaySync = Substitute.For<IDisplaySyncService>();

	private IRenderedComponent<CyclePosition> RenderPosition(int length, int displayIndex)
	{
		_displaySync.DisplayIndex.Returns(displayIndex);
		Services.AddSingleton(_displaySync);

		return Render<CyclePosition>(parameters => parameters.Add(position => position.Length, length));
	}

	[TestMethod]
	public void Render_ShowsOneBasedPositionOfCycle()
	{
		var position = RenderPosition(length: 20, displayIndex: 2);

		Assert.AreEqual("3 / 20", position.Markup);
	}

	[TestMethod]
	public void Render_ShorterCycle_WrapsPosition()
	{
		var position = RenderPosition(length: 3, displayIndex: 7);

		Assert.AreEqual("2 / 3", position.Markup);
	}

	[TestMethod]
	public void Render_EmptyCycle_ShowsNothing()
	{
		var position = RenderPosition(length: 0, displayIndex: 4);

		Assert.AreEqual(string.Empty, position.Markup);
	}

	[TestMethod]
	public void DisplayChanged_UpdatesPosition()
	{
		var position = RenderPosition(length: 5, displayIndex: 0);
		_displaySync.DisplayIndex.Returns(1);

		_displaySync.DisplayChanged += Raise.Event();

		position.WaitForAssertion(() => Assert.AreEqual("2 / 5", position.Markup));
	}

	[TestMethod]
	public async Task Dispose_RemovesDisplayChangedSubscription()
	{
		var position = RenderPosition(length: 5, displayIndex: 0);
		await DisposeComponentsAsync();
		_displaySync.DisplayIndex.Returns(3);

		_displaySync.DisplayChanged += Raise.Event();

		Assert.AreEqual("1 / 5", position.Markup);
	}
}
