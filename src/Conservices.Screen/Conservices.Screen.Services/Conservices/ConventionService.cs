using Conservices.Screen.Interfaces.Conservices;
using Conservices.Screen.Interfaces.Repositories;
using Conservices.Screen.Models.Convention;
using Microsoft.Extensions.Caching.Memory;

namespace Conservices.Screen.Services.Conservices;

public class ConventionService(IConventionRepository conventionRepository, IMemoryCache cache)
	: AbstractKeyedConService<Convention>(cache), IConventionService
{
	protected override TimeSpan RefreshInterval => TimeSpan.FromHours(1);

	protected override Task<IEnumerable<Convention>> GetItemsForConventionAsync(string eventId)
		=> conventionRepository.GetAllAsync();

	public async Task<IEnumerable<Convention>> GetAllAsync() => await GetItemsAsync();

	public async Task<Convention?> GetAsync(string id)
	{
		var items = await GetItemsAsync();
		return items.FirstOrDefault(x => x.Id == id);
	}
}