using Microsoft.AspNetCore.Components;
using Questlog.Core.Time;

namespace Questlog.Ui.Components;

public partial class Clock : ComponentBase, IDisposable
{
	private ITimer? _timer;

	[Inject]
	private TimeProvider Time { get; set; } = null!;

	[Inject]
	private IDisplayTimeFormatter Formatter { get; set; } = null!;

	protected override void OnInitialized()
		=> _timer = Time.CreateTimer(_ => InvokeAsync(StateHasChanged), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

	/// <summary>
	/// Stops the timer which refreshes the clock.
	/// </summary>
	public void Dispose()
		=> _timer?.Dispose();
}
