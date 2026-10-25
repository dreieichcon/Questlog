using System.Collections.Concurrent;
using Demolite.Core.Models;

namespace Questlog.Services;

public abstract class CachedValueService<TValue>(TimeProvider timeProvider, TimeSpan maxAge)
{
	private readonly ConcurrentDictionary<string, CacheSlot> _slots = new();

	/// <summary>
	/// The current time in UTC.
	/// </summary>
	protected DateTimeOffset UtcNow => timeProvider.GetUtcNow();

	/// <summary>
	/// Returns the cached value for the key. Fetches it if it is older than the maximum age.
	/// A failed fetch counts as an attempt and returns the last known value, or the fallback.
	/// </summary>
	protected async Task<TValue> GetCachedAsync(
		string key,
		Func<CancellationToken, Task<Result<TValue>>> fetch,
		TValue fallback,
		CancellationToken ct)
	{
		var slot = _slots.GetOrAdd(key, _ => new CacheSlot());

		if (IsFresh(slot.Entry))
			return slot.Entry!.Value;

		await slot.Lock.WaitAsync(ct);

		try
		{
			if (IsFresh(slot.Entry))
				return slot.Entry!.Value;

			var result = await fetch(ct);

			var value = result.IsSuccess ? result.Value! : slot.Entry is null ? fallback : slot.Entry.Value;
			slot.Entry = new CacheEntry(value, UtcNow);

			return value;
		}
		finally
		{
			slot.Lock.Release();
		}
	}

	private bool IsFresh(CacheEntry? entry)
		=> entry is not null && UtcNow - entry.AttemptedAt < maxAge;

	private sealed record CacheEntry(TValue Value, DateTimeOffset AttemptedAt);

	private sealed class CacheSlot
	{
		public SemaphoreSlim Lock { get; } = new(1, 1);

		public volatile CacheEntry? Entry;
	}
}
