using Microsoft.AspNetCore.Components;

namespace Questlog.Ui.Components.Cells;

public partial class TitleCell : ComponentBase
{
	[Parameter, EditorRequired]
	public string Title { get; set; } = string.Empty;

	[Parameter]
	public int? MinimumAge { get; set; }

	private string? AgeBadge => MinimumAge switch
	{
		>= 18 => "18",
		>= 16 => "16",
		_ => null,
	};

	private string AgeClass => $"age-{AgeBadge}";
}
