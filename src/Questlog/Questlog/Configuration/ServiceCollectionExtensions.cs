using Demolite.Http.Extensions;
using Questlog.Core.Interfaces;
using Questlog.Core.Time;
using Questlog.Repositories.Conservices;
using Questlog.Repositories.Sun;
using Questlog.Services;

namespace Questlog.Configuration;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers the repositories, services, timers and formatting of the application.
	/// </summary>
	public static IServiceCollection AddQuestlog(this IServiceCollection services, QuestlogSettings settings)
	{
		services.AddSingleton(TimeProvider.System);
		services.AddSingleton(settings.TimeZone);
		services.AddSingleton(settings.Location);
		services.AddSingleton<IDisplayTimeFormatter, DisplayTimeFormatter>();

		return services
			.AddRepositories()
			.AddSharedServices();
	}

	private static IServiceCollection AddRepositories(this IServiceCollection services)
	{
		services.AddApiClient<ConservicesRepository>(ConservicesRepository.BaseAddress);
		services.AddTransient<IConservicesRepository>(provider => provider.GetRequiredService<ConservicesRepository>());

		services.AddApiClient<SunRepository>(SunRepository.BaseAddress);
		services.AddTransient<ISunRepository>(provider => provider.GetRequiredService<SunRepository>());

		return services;
	}

	private static IServiceCollection AddSharedServices(this IServiceCollection services)
	{
		services.AddSingleton<ITimerService, TimerService>();
		services.AddSingleton<IDisplaySyncService, DisplaySyncService>();
		services.AddSingleton<IConventionService, ConventionService>();
		services.AddSingleton<IGameService, GameService>();
		services.AddSingleton<IProgramService, ProgramService>();
		services.AddSingleton<IThemeService, ThemeService>();

		return services;
	}
}
