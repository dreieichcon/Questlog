using Microsoft.AspNetCore.Components;
using Questlog.Core.Models;

namespace Questlog.Ui.Components.Cells;

public partial class TagCell : ComponentBase
{
	[Parameter]
	public IEnumerable<Tag> Tags { get; set; } = [];
}
