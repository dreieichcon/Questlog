using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Services;

public class ConventionService(IConservicesRepository repository, TimeProvider timeProvider)
	: CachedListService<Convention>(timeProvider, TimeSpan.FromHours(1)), IConventionService
{
	private const string CacheKey = "conventions";

	/// <summary>
	/// Returns all conventions.
	/// </summary>
	public Task<IReadOnlyList<Convention>> GetAllAsync(CancellationToken ct = default)
		=> GetCachedAsync(CacheKey, repository.GetConventionsAsync, ct);

	/// <summary>
	/// Returns the convention with the id, or null if it does not exist.
	/// </summary>
	public async Task<Convention?> GetAsync(string id, CancellationToken ct = default)
	{
		var conventions = await GetAllAsync(ct);
		return conventions.FirstOrDefault(x => x.Id == id);
	}
}
