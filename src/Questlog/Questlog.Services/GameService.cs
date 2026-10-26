using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Core.Resources;

namespace Questlog.Services;

public class GameService(IConservicesRepository repository, TimeProvider timeProvider)
	: CachedValueService<IReadOnlyList<Game>>(timeProvider, TimeSpan.FromMinutes(5)), IGameService
{
	private const int MinimumSystemGames = 4;

	/// <summary>
	/// Returns all games of the convention.
	/// </summary>
	public Task<IReadOnlyList<Game>> GetAllAsync(string eventId, CancellationToken ct = default)
		=> GetCachedAsync(eventId, token => repository.GetGamesAsync(eventId, token), [], ct);

	/// <summary>
	/// Returns the distinct building names of all game tables that have a location.
	/// </summary>
	public async Task<IReadOnlyList<string>> GetBuildingsAsync(string eventId, CancellationToken ct = default)
	{
		var games = await GetAllAsync(eventId, ct);

		return games
			.SelectMany(game => game.Tables)
			.Select(table => table.Location?.Building)
			.OfType<string>()
			.Distinct()
			.ToList();
	}

	/// <summary>
	/// Returns the number of games per system, ignoring versions.
	/// Systems with fewer games than the minimum are merged into one trailing slice.
	/// </summary>
	public async Task<IReadOnlyList<SystemShare>> GetSystemSharesAsync(string eventId, CancellationToken ct = default)
	{
		var games = await GetAllAsync(eventId, ct);

		var shares = games
			.GroupBy(game => NormalizeSystemName(game.System), StringComparer.OrdinalIgnoreCase)
			.Select(group => new SystemShare(group.Key, group.Count()))
			.ToList();

		var large = shares.Where(share => share.Count >= MinimumSystemGames).OrderByDescending(share => share.Count);
		var small = shares.Where(share => share.Count < MinimumSystemGames).Sum(share => share.Count);

		return small == 0
			? [.. large]
			: [.. large, new SystemShare(Strings.Get("System_Other"), small)];
	}

	private static string NormalizeSystemName(string system)
		=> string.IsNullOrWhiteSpace(system) ? Strings.Get("System_Unknown") : system.Trim();
}
