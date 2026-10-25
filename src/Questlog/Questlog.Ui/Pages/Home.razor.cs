using Microsoft.AspNetCore.Components;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Ui.Pages;

public partial class Home : ComponentBase
{
	[Inject]
	private IConventionService ConventionService { get; set; } = null!;

	private IReadOnlyList<Convention> Conventions { get; set; } = [];

	protected override async Task OnInitializedAsync()
		=> Conventions = await ConventionService.GetAllAsync();
}
