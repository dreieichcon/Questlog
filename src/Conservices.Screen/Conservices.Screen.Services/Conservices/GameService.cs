using Conservices.Screen.Interfaces.Conservices;
using Conservices.Screen.Interfaces.Repositories;
using Conservices.Screen.Models.Games;
using Conservices.Screen.Models.Misc;
using Microsoft.Extensions.Caching.Memory;

namespace Conservices.Screen.Services.Conservices;

public class GameService(IGameRepository gameRepository, IMemoryCache cache)
	: AbstractKeyedConService<Game>(cache), IGameService
{
	protected override TimeSpan RefreshInterval => TimeSpan.FromMinutes(5);

	protected override Task<IEnumerable<Game>> GetItemsForConventionAsync(string eventId)
		=> gameRepository.GetAllAsync(eventId);

	public Task<IEnumerable<Game>> GetAllAsync(string eventId) => GetItemsAsync(eventId);

	public async Task<IEnumerable<Location>> GetAllBuildings(string eventId)
	{
		var items = await GetItemsAsync(eventId);
		return items.SelectMany(x => x.Tables.Select(y => y.Location)).DistinctBy(x => x.Building);
	}
}