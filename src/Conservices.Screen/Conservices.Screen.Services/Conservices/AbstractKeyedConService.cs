using Microsoft.Extensions.Caching.Memory;

namespace Conservices.Screen.Services.Conservices;

public abstract class AbstractKeyedConService<T>(IMemoryCache cache)
{
	protected abstract TimeSpan RefreshInterval { get; }

	protected abstract Task<IEnumerable<T>> GetItemsForConventionAsync(string eventId);

	protected async Task<IEnumerable<T>> GetItemsAsync(string eventId)
	{
		return await cache.GetOrCreateAsync(CacheKey(eventId), async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = RefreshInterval;
			return await GetItemsForConventionAsync(eventId);
		}) ?? [];
	}

	private static string CacheKey(string eventId) => $"{typeof(T).Name}:{eventId}";
}