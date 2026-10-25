using Microsoft.Extensions.Time.Testing;

namespace Questlog.Services.Tests;

[TestClass]
public class TimerServiceTests
{
	private readonly FakeTimeProvider _timeProvider = new(new DateTimeOffset(2030, 11, 14, 10, 0, 0, TimeSpan.Zero));

	private TimerService CreateService() => new(_timeProvider);

	private void AdvanceSeconds(int seconds) => _timeProvider.Advance(TimeSpan.FromSeconds(seconds));

	[TestMethod]
	public void TickItem_DoesNotFireBeforeFiveSeconds()
	{
		using var service = CreateService();
		var count = 0;
		service.TickItem += (_, _) => count++;

		AdvanceSeconds(4);

		Assert.AreEqual(0, count);
	}

	[TestMethod]
	public void TickItem_FiresEveryFiveSeconds()
	{
		using var service = CreateService();
		var count = 0;
		service.TickItem += (_, _) => count++;

		AdvanceSeconds(5);
		Assert.AreEqual(1, count);

		AdvanceSeconds(20);
		Assert.AreEqual(5, count);
	}

	[TestMethod]
	public void TickPage_DoesNotFireBeforeTwentyItems()
	{
		using var service = CreateService();
		var count = 0;
		service.TickPage += (_, _) => count++;

		AdvanceSeconds(95);

		Assert.AreEqual(0, count);
	}

	[TestMethod]
	public void TickPage_FiresEveryHundredSeconds()
	{
		using var service = CreateService();
		var count = 0;
		service.TickPage += (_, _) => count++;

		AdvanceSeconds(100);
		Assert.AreEqual(1, count);

		AdvanceSeconds(100);
		Assert.AreEqual(2, count);
	}

	[TestMethod]
	public void TickPage_FiresAfterTheItemTickOfTheSameMoment()
	{
		using var service = CreateService();
		var order = new List<string>();
		service.TickItem += (_, _) => order.Add("item");
		service.TickPage += (_, _) => order.Add("page");

		AdvanceSeconds(100);

		Assert.AreEqual("page", order[^1]);
		Assert.AreEqual("item", order[^2]);
		Assert.AreEqual(1, order.Count(x => x == "page"));
	}

	[TestMethod]
	public void TickMinute_FiresOnlyEverySixtySeconds()
	{
		using var service = CreateService();
		var count = 0;
		service.TickMinute += (_, _) => count++;

		AdvanceSeconds(59);
		Assert.AreEqual(0, count);

		AdvanceSeconds(1);
		Assert.AreEqual(1, count);

		AdvanceSeconds(60);
		Assert.AreEqual(2, count);
	}

	[TestMethod]
	public void ThrowingHandler_DoesNotStopOtherHandlersOrLaterTicks()
	{
		using var service = CreateService();
		var count = 0;
		service.TickItem += (_, _) => throw new InvalidOperationException("handler failed");
		service.TickItem += (_, _) => count++;

		AdvanceSeconds(10);

		Assert.AreEqual(2, count);
	}

	[TestMethod]
	public void Dispose_StopsAllTicks()
	{
		var service = CreateService();
		var count = 0;
		service.TickItem += (_, _) => count++;
		service.TickPage += (_, _) => count++;
		service.TickMinute += (_, _) => count++;

		service.Dispose();
		AdvanceSeconds(300);

		Assert.AreEqual(0, count);
	}
}
