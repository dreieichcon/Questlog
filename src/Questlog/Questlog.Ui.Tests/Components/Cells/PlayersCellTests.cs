using Bunit;
using Questlog.Ui.Components.Cells;

namespace Questlog.Ui.Tests.Components.Cells;

[TestClass]
public class PlayersCellTests : BunitContext
{
	private string RenderMarkup(int current, int max)
		=> Render<PlayersCell>(parameters => parameters
			.Add(cell => cell.Current, current)
			.Add(cell => cell.Max, max)).Markup;

	[TestMethod]
	public void Render_ShowsCurrentAndMax()
	{
		StringAssert.Contains(RenderMarkup(2, 4), "2 / 4");
	}

	[TestMethod]
	public void Render_FreeSeats_IsMarkedAsFree()
	{
		StringAssert.Contains(RenderMarkup(3, 4), "seats-free");
	}

	[TestMethod]
	public void Render_FullGame_IsNotMarkedAsFree()
	{
		Assert.IsFalse(RenderMarkup(4, 4).Contains("seats-free"));
	}
}
