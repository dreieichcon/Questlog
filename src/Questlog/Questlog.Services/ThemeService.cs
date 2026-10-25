using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Services;

public class ThemeService(ISunRepository repository, TimeProvider timeProvider)
	: CachedValueService<SunState>(timeProvider, TimeSpan.FromDays(1)), IThemeService
{
	private const string CacheKey = "sun";

	private static readonly TimeSpan DarkBeforeSunset = TimeSpan.FromHours(1);

	private static readonly TimeSpan DarkAfterSunrise = TimeSpan.FromHours(1.5);

	// Conventions take place in October and November, where Dreieich has these approximate UTC times.
	private static readonly SunState FallbackState = new() { Sunrise = new TimeOnly(6, 0), Sunset = new TimeOnly(16, 0) };

	/// <summary>
	/// Returns whether the dark theme applies now. Falls back to fixed times if no sun data is known.
	/// </summary>
	public async Task<bool> IsDarkAsync(CancellationToken ct = default)
	{
		var sun = await GetCachedAsync(CacheKey, repository.GetSunStateAsync, FallbackState, ct);
		var now = TimeOnly.FromDateTime(UtcNow.UtcDateTime);

		return now >= sun.Sunset.Add(-DarkBeforeSunset) || now <= sun.Sunrise.Add(DarkAfterSunrise);
	}
}
