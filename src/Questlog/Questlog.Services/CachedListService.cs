using System.Collections.Concurrent;
using Demolite.Core.Models;

namespace Questlog.Services;

public abstract class CachedListService<T>(TimeProvider timeProvider, TimeSpan maxAge)
{
	private readonly ConcurrentDictionary<string, CacheSlot> _slots = new();

	/// <summary>
	/// Returns the cached items for the key. Fetches them if they are older than the maximum age.
	/// A failed fetch counts as an attempt and returns the last known items, or an empty list.
	/// </summary>
	protected async Task<IReadOnlyList<T>> GetCachedAsync(
		string key,
		Func<CancellationToken, Task<Result<IReadOnlyList<T>>>> fetch,
		CancellationToken ct)
	{
		var slot = _slots.GetOrAdd(key, _ => new CacheSlot());

		if (IsFresh(slot.Entry))
			return slot.Entry!.Items;

		await slot.Lock.WaitAsync(ct);

		try
		{
			if (IsFresh(slot.Entry))
				return slot.Entry!.Items;

			var result = await fetch(ct);

			var items = result.IsSuccess ? result.Value! : slot.Entry?.Items ?? [];
			slot.Entry = new CacheEntry(items, timeProvider.GetUtcNow());

			return items;
		}
		finally
		{
			slot.Lock.Release();
		}
	}

	private bool IsFresh(CacheEntry? entry)
		=> entry is not null && timeProvider.GetUtcNow() - entry.AttemptedAt < maxAge;

	private sealed record CacheEntry(IReadOnlyList<T> Items, DateTimeOffset AttemptedAt);

	private sealed class CacheSlot
	{
		public SemaphoreSlim Lock { get; } = new(1, 1);

		public volatile CacheEntry? Entry;
	}
}
