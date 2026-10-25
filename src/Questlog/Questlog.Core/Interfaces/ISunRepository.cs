using Demolite.Core.Models;
using Questlog.Core.Models;

namespace Questlog.Core.Interfaces;

public interface ISunRepository
{
	public Task<Result<SunState>> GetSunStateAsync(CancellationToken ct = default);
}
