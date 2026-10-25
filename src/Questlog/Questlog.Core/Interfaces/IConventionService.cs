using Questlog.Core.Models;

namespace Questlog.Core.Interfaces;

public interface IConventionService
{
	public Task<IReadOnlyList<Convention>> GetAllAsync(CancellationToken ct = default);

	public Task<Convention?> GetAsync(string id, CancellationToken ct = default);
}
