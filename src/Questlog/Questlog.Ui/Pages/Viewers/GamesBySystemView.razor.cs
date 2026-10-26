using Microsoft.AspNetCore.Components;
using MudBlazor;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Core.Resources;

namespace Questlog.Ui.Pages.Viewers;

public partial class GamesBySystemView : ComponentBase
{
	private const string OtherSliceColor = "#8A8A8A";

	private static readonly string[] SlicePalette =
	[
		"#0892A5", "#F2A900", "#7B5EA7", "#E4572E", "#4CAF50", "#2E86DE",
		"#D81B60", "#8D6E63", "#26A69A", "#C0CA33", "#5C6BC0", "#FF7043",
	];

	private string? _loadedEventId;

	private int _total;

	private IReadOnlyList<SystemShare> _shares = [];

	private List<ChartSeries<double>> _series = [];

	private string[] _labels = [];

	private string[] _colors = [];

	private ChartOptions _options = new();

	[Inject]
	private IGameService GameService { get; set; } = null!;

	[Parameter]
	public string EventId { get; set; } = string.Empty;

	protected override async Task OnParametersSetAsync()
	{
		if (EventId == _loadedEventId)
			return;

		_loadedEventId = EventId;
		_shares = await GameService.GetSystemSharesAsync(EventId);
		_total = _shares.Sum(share => share.Count);
		_labels = [.. _shares.Select(share => $"{share.Name}: {FormatValue(share)}")];
		_colors = CreateColors();
		var counts = _shares.Select(share => (double)share.Count).ToList();
		_series = [new ChartSeries<double> { Name = Strings.Get("View_GamesBySystem"), Data = new ChartData<double>(counts) }];
		_options = new ChartOptions { ChartPalette = _colors, ShowLegend = false };
	}

	private string FormatValue(SystemShare share)
		=> $"{share.Count} ({(double)share.Count / _total:P0})";

	private string[] CreateColors()
		=> [.. _shares.Select((share, index) => IsOtherSlice(share) ? OtherSliceColor : SlicePalette[index % SlicePalette.Length])];

	private static bool IsOtherSlice(SystemShare share)
		=> share.Name == Strings.Get("System_Other");
}
