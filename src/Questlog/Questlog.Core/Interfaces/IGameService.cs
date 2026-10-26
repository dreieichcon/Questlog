using Questlog.Core.Models;

namespace Questlog.Core.Interfaces;

public interface IGameService
{
	public Task<IReadOnlyList<Game>> GetAllAsync(string eventId, CancellationToken ct = default);

	public Task<IReadOnlyList<string>> GetBuildingsAsync(string eventId, CancellationToken ct = default);

	public Task<IReadOnlyList<SystemShare>> GetSystemSharesAsync(string eventId, CancellationToken ct = default);
}
