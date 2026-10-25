namespace Questlog.Core.Interfaces;

public interface IThemeService
{
	public Task<bool> IsDarkAsync(CancellationToken ct = default);
}
