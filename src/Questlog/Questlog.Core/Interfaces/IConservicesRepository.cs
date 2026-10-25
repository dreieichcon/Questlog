using Demolite.Core.Models;
using Questlog.Core.Models;

namespace Questlog.Core.Interfaces;

public interface IConservicesRepository
{
	public Task<Result<IReadOnlyList<Convention>>> GetConventionsAsync(CancellationToken ct = default);

	public Task<Result<IReadOnlyList<Game>>> GetGamesAsync(string eventId, CancellationToken ct = default);

	public Task<Result<IReadOnlyList<ProgramItem>>> GetProgramItemsAsync(string eventId, CancellationToken ct = default);
}
