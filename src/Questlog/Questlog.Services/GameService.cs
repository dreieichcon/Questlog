using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Services;

public class GameService(IConservicesRepository repository, TimeProvider timeProvider)
	: CachedValueService<IReadOnlyList<Game>>(timeProvider, TimeSpan.FromMinutes(5)), IGameService
{
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
}
