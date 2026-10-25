using Conservices.Screen.Interfaces.Timers;
using Conservices.Screen.Models.Enum;
using Conservices.Screen.Services.Display;
using NSubstitute;

namespace Conservices.Screen.Services.Tests;

[TestClass]
public class DisplaySyncServiceTests
{
	private readonly ITimerService _timer = Substitute.For<ITimerService>();

	private DisplaySyncService CreateService() => new(_timer);

	private void RaiseItemTicks(int count)
	{
		for (var i = 0; i < count; i++)
			_timer.TickItem += Raise.Event();
	}

	private void RaisePageTick()
		=> _timer.TickPage += Raise.Event();

	[TestMethod]
	public void NewService_StartsWithProgramAtFirstIndex()
	{
		var service = CreateService();

		Assert.AreEqual(ActiveDisplay.Program, service.ActiveDisplay);
		Assert.AreEqual(0, service.DisplayIndex);
	}

	[TestMethod]
	public void MaxIndex_LimitsVisibleEntriesToTwenty()
	{
		var service = CreateService();

		Assert.AreEqual(20, service.MaxIndex);
	}

	[TestMethod]
	public void TickItem_IncrementsDisplayIndex()
	{
		var service = CreateService();

		RaiseItemTicks(3);

		Assert.AreEqual(3, service.DisplayIndex);
	}

	[TestMethod]
	public void TickItem_RaisesDisplayChangedOncePerTick()
	{
		var service = CreateService();
		var raised = 0;
		service.DisplayChanged += (_, _) => raised++;

		RaiseItemTicks(2);

		Assert.AreEqual(2, raised);
	}

	[TestMethod]
	public void TickItem_OnLastIndex_WrapsToZero()
	{
		var service = CreateService();

		RaiseItemTicks(service.MaxIndex - 1);
		Assert.AreEqual(service.MaxIndex - 1, service.DisplayIndex);

		RaiseItemTicks(1);
		Assert.AreEqual(0, service.DisplayIndex);
	}

	[TestMethod]
	public void TickItem_DoesNotRaisePageChanged()
	{
		var service = CreateService();
		var raised = false;
		service.PageChanged += (_, _) => raised = true;

		RaiseItemTicks(1);

		Assert.IsFalse(raised);
	}

	[TestMethod]
	public void TickPage_SwitchesBetweenProgramAndGames()
	{
		var service = CreateService();

		RaisePageTick();
		Assert.AreEqual(ActiveDisplay.Games, service.ActiveDisplay);

		RaisePageTick();
		Assert.AreEqual(ActiveDisplay.Program, service.ActiveDisplay);
	}

	[TestMethod]
	public void TickPage_RaisesPageChanged()
	{
		var service = CreateService();
		var raised = 0;
		service.PageChanged += (_, _) => raised++;

		RaisePageTick();

		Assert.AreEqual(1, raised);
	}

	[TestMethod]
	public void TickPage_DoesNotRaiseDisplayChanged()
	{
		var service = CreateService();
		var raised = false;
		service.DisplayChanged += (_, _) => raised = true;

		RaisePageTick();

		Assert.IsFalse(raised);
	}
}
