using Microsoft.AspNetCore.Components;
using Questlog.Core.Models;

namespace Questlog.Ui.Components.Cells;

public partial class LocationCell : ComponentBase
{
	[Parameter]
	public IEnumerable<Table> Tables { get; set; } = [];

	private string TableName => Tables.FirstOrDefault()?.Name ?? string.Empty;

	private bool IsAccessible => Tables.Any(table => table.Accessible);
}
