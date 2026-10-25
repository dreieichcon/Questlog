using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Questlog.Configuration;
using Questlog.Core.Interfaces;
using Questlog.Core.Time;

namespace Questlog.Tests;

[TestClass]
public class ServiceRegistrationTests
{
	private static ServiceProvider BuildProvider()
	{
		var settings = QuestlogSettings.FromConfiguration(new ConfigurationBuilder().Build());
		var services = new ServiceCollection();
		services.AddLogging();
		services.AddQuestlog(settings);

		return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
	}

	[TestMethod]
	[DataRow(typeof(IConservicesRepository))]
	[DataRow(typeof(ISunRepository))]
	[DataRow(typeof(IConventionService))]
	[DataRow(typeof(IGameService))]
	[DataRow(typeof(IProgramService))]
	[DataRow(typeof(IThemeService))]
	[DataRow(typeof(ITimerService))]
	[DataRow(typeof(IDisplaySyncService))]
	[DataRow(typeof(IDisplayTimeFormatter))]
	public void EveryApplicationService_CanBeResolved(Type serviceType)
	{
		using var provider = BuildProvider();

		Assert.IsNotNull(provider.GetRequiredService(serviceType));
	}

	[TestMethod]
	[DataRow(typeof(IConventionService))]
	[DataRow(typeof(IGameService))]
	[DataRow(typeof(IProgramService))]
	[DataRow(typeof(IThemeService))]
	[DataRow(typeof(ITimerService))]
	[DataRow(typeof(IDisplaySyncService))]
	public void SharedServices_AreSingletons(Type serviceType)
	{
		using var provider = BuildProvider();
		using var scope = provider.CreateScope();

		var first = provider.GetRequiredService(serviceType);
		var second = scope.ServiceProvider.GetRequiredService(serviceType);

		Assert.AreSame(first, second);
	}

	[TestMethod]
	public void TimeProvider_IsTheSystemClock()
	{
		using var provider = BuildProvider();

		Assert.AreSame(TimeProvider.System, provider.GetRequiredService<TimeProvider>());
	}

	[TestMethod]
	public void TimeZoneAndLocation_ComeFromSettings()
	{
		using var provider = BuildProvider();

		Assert.AreEqual("Europe/Berlin", provider.GetRequiredService<TimeZoneInfo>().Id);
		Assert.AreEqual(50.02485985471239, provider.GetRequiredService<Core.Configuration.GeoLocation>().Latitude);
	}
}
