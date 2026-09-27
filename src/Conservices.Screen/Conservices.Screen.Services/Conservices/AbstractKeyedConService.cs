using Microsoft.Extensions.Caching.Memory;

namespace Conservices.Screen.Services.Conservices;

public abstract class AbstractKeyedConService<T>(IMemoryCache cache)
{
	protected abstract TimeSpan RefreshInterval { get; }

	protected abstract Task<IEnumerable<T>> GetItemsForConventionAsync(string eventId);

	protected virtual string BuildCacheKey(string eventId) => $"{typeof(T).Name}:{eventId}";

	protected async Task<IEnumerable<T>> GetItemsAsync(string eventId = "all")
	{
		return await cache.GetOrCreateAsync(BuildCacheKey(eventId), async entry =>
		{
			entry.AbsoluteExpirationRelativeToNow = RefreshInterval;
			return await GetItemsForConventionAsync(eventId);
		}) ?? [];
	}
}