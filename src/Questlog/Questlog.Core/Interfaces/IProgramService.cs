using Questlog.Core.Models;

namespace Questlog.Core.Interfaces;

public interface IProgramService
{
	public Task<IReadOnlyList<ProgramItem>> GetAllAsync(string eventId, CancellationToken ct = default);
}
