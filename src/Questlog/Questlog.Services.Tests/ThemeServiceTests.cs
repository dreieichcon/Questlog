using Demolite.Core.Models;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Services.Tests;

[TestClass]
public class ThemeServiceTests
{
	private readonly ISunRepository _repository = Substitute.For<ISunRepository>();

	private readonly FakeTimeProvider _timeProvider = new();

	private ThemeService CreateService() => new(_repository, _timeProvider);

	private void SetupSun(TimeOnly sunrise, TimeOnly sunset)
		=> _repository.GetSunStateAsync(Arg.Any<CancellationToken>())
			.Returns(Result<SunState>.Success(new SunState { Sunrise = sunrise, Sunset = sunset }));

	private void SetupFailure()
		=> _repository.GetSunStateAsync(Arg.Any<CancellationToken>())
			.Returns(Result<SunState>.Failure(new Error("Test.Failure", "failed")));

	private void SetUtcTime(int hour, int minute, int second = 0)
		=> _timeProvider.SetUtcNow(new DateTimeOffset(2030, 11, 14, hour, minute, second, TimeSpan.Zero));

	[TestMethod]
	[DataRow(3, 0, true)]
	[DataRow(7, 30, true)]
	[DataRow(7, 31, false)]
	[DataRow(12, 0, false)]
	[DataRow(15, 17, false)]
	[DataRow(15, 18, true)]
	[DataRow(23, 0, true)]
	public async Task IsDarkAsync_WithSunData_IsDarkFromOneHourBeforeSunsetUntilNinetyMinutesAfterSunrise(int hour, int minute, bool expected)
	{
		SetupSun(new TimeOnly(6, 0, 43), new TimeOnly(16, 17, 24));
		SetUtcTime(hour, minute);
		var service = CreateService();

		var result = await service.IsDarkAsync();

		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	[DataRow(7, 30, true)]
	[DataRow(7, 31, false)]
	[DataRow(14, 59, false)]
	[DataRow(15, 0, true)]
	public async Task IsDarkAsync_WithoutSunData_UsesFallbackSunriseSixAndSunsetSixteenUtc(int hour, int minute, bool expected)
	{
		SetupFailure();
		SetUtcTime(hour, minute);
		var service = CreateService();

		var result = await service.IsDarkAsync();

		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	public async Task IsDarkAsync_UsesLatestClockTimeOnEveryCall()
	{
		SetupSun(new TimeOnly(6, 0), new TimeOnly(16, 0));
		var service = CreateService();

		SetUtcTime(12, 0);
		Assert.IsFalse(await service.IsDarkAsync());

		SetUtcTime(20, 0);
		Assert.IsTrue(await service.IsDarkAsync());
	}

	[TestMethod]
	public async Task IsDarkAsync_WithinOneDay_QueriesRepositoryOnce()
	{
		SetupSun(new TimeOnly(6, 0), new TimeOnly(16, 0));
		SetUtcTime(12, 0);
		var service = CreateService();

		await service.IsDarkAsync();
		_timeProvider.Advance(TimeSpan.FromHours(23));
		await service.IsDarkAsync();

		await _repository.Received(1).GetSunStateAsync(Arg.Any<CancellationToken>());
	}

	[TestMethod]
	public async Task IsDarkAsync_AfterOneDay_QueriesRepositoryAgain()
	{
		SetupSun(new TimeOnly(6, 0), new TimeOnly(16, 0));
		SetUtcTime(12, 0);
		var service = CreateService();

		await service.IsDarkAsync();
		_timeProvider.Advance(TimeSpan.FromHours(25));
		await service.IsDarkAsync();

		await _repository.Received(2).GetSunStateAsync(Arg.Any<CancellationToken>());
	}

	[TestMethod]
	public async Task IsDarkAsync_FetchFailsAfterSuccess_KeepsLastKnownSunData()
	{
		SetupSun(new TimeOnly(4, 0), new TimeOnly(20, 0));
		SetUtcTime(12, 0);
		var service = CreateService();
		await service.IsDarkAsync();

		_timeProvider.Advance(TimeSpan.FromHours(25));
		SetupFailure();
		_timeProvider.Advance(TimeSpan.FromHours(3.5));

		Assert.IsFalse(await service.IsDarkAsync());
	}
}
