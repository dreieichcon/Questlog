using Microsoft.AspNetCore.Components;
using Questlog.Core.Interfaces;

namespace Questlog.Ui.Components;

public partial class ScreenFrame : ComponentBase, IDisposable
{
	private bool _isDark;

	[Inject]
	private IThemeService ThemeService { get; set; } = null!;

	[Inject]
	private ITimerService TimerService { get; set; } = null!;

	[Parameter, EditorRequired]
	public string Title { get; set; } = string.Empty;

	[Parameter]
	public string? LogoUrl { get; set; }

	[Parameter]
	public RenderFragment? FooterLeft { get; set; }

	[Parameter]
	public RenderFragment? FooterRight { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private string ThemeClass => _isDark ? "theme-dark" : string.Empty;

	protected override async Task OnInitializedAsync()
	{
		_isDark = await ThemeService.IsDarkAsync();
		TimerService.TickMinute += OnTickMinute;
	}

	/// <summary>
	/// Removes the subscription to the minute tick.
	/// </summary>
	public void Dispose()
		=> TimerService.TickMinute -= OnTickMinute;

	private void OnTickMinute(object? sender, EventArgs e)
		=> _ = InvokeAsync(RefreshThemeAsync);

	private async Task RefreshThemeAsync()
	{
		_isDark = await ThemeService.IsDarkAsync();
		StateHasChanged();
	}
}
