using Microsoft.AspNetCore.Components;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Core.Resources;

namespace Questlog.Ui.Pages.Viewers;

public partial class ProgramAndGamesView : ComponentBase, IDisposable
{
	private int _programLength;

	private int _gamesLength;

	[Inject]
	private IDisplaySyncService DisplaySyncService { get; set; } = null!;

	[Parameter]
	public string EventId { get; set; } = string.Empty;

	private ActiveDisplay ShownDisplay => SkipEmptyDisplay(DisplaySyncService.ActiveDisplay);

	private string ViewName => Strings.Get(ShownDisplay == ActiveDisplay.Program ? "View_Program" : "View_Games");

	private int ShownLength => LengthOf(ShownDisplay);

	protected override void OnInitialized()
		=> DisplaySyncService.PageChanged += OnPageChanged;

	/// <summary>
	/// Removes the subscription to the page changes.
	/// </summary>
	public void Dispose()
		=> DisplaySyncService.PageChanged -= OnPageChanged;

	private void OnProgramLengthChanged(int length)
	{
		_programLength = length;
	}

	private void OnGamesLengthChanged(int length)
	{
		_gamesLength = length;
	}

	private void OnPageChanged(object? sender, EventArgs e)
		=> _ = InvokeAsync(StateHasChanged);

	private bool IsShown(ActiveDisplay display)
		=> display == ShownDisplay;

	private int LengthOf(ActiveDisplay display)
		=> display == ActiveDisplay.Program ? _programLength : _gamesLength;

	private ActiveDisplay SkipEmptyDisplay(ActiveDisplay active)
	{
		var other = active == ActiveDisplay.Program ? ActiveDisplay.Games : ActiveDisplay.Program;

		return LengthOf(active) == 0 && LengthOf(other) > 0 ? other : active;
	}
}
