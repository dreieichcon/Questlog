using Microsoft.AspNetCore.Components;
using Questlog.Core.Text;

namespace Questlog.Ui.Components.Cells;

public partial class BreakableText : ComponentBase
{
	[Parameter]
	public string Text { get; set; } = string.Empty;

	private IReadOnlyList<string> Parts => WordBreaker.Split(Text);
}
