using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Services;

public class ProgramService(IConservicesRepository repository, TimeProvider timeProvider)
	: CachedListService<ProgramItem>(timeProvider, TimeSpan.FromMinutes(5)), IProgramService
{
	/// <summary>
	/// Returns all program items of the convention.
	/// </summary>
	public Task<IReadOnlyList<ProgramItem>> GetAllAsync(string eventId, CancellationToken ct = default)
		=> GetCachedAsync(eventId, token => repository.GetProgramItemsAsync(eventId, token), ct);
}
