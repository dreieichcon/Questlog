using Microsoft.AspNetCore.Components;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Core.Resources;
using Questlog.Core.Time;

namespace Questlog.Ui.Pages;

public partial class Event : ComponentBase
{
	[Inject]
	private IConventionService ConventionService { get; set; } = null!;

	[Inject]
	private IDisplayTimeFormatter Formatter { get; set; } = null!;

	[Parameter]
	public string EventId { get; set; } = string.Empty;

	private Convention? Convention { get; set; }

	private bool IsLoaded { get; set; }

	private IEnumerable<ViewOption> Views => Convention is null ? [] : CreateViews(Convention);

	protected override async Task OnParametersSetAsync()
	{
		Convention = await ConventionService.GetAsync(EventId);
		IsLoaded = true;
	}

	private IEnumerable<ViewOption> CreateViews(Convention convention)
	{
		if (convention.HasProgramModule)
		{
			yield return CreateView("View_Program", "program");
			yield return CreateView("View_ProgramPerRoom", "program/rooms");
		}

		if (convention.HasGameModule)
		{
			yield return CreateView("View_Games", "games");
			yield return CreateView("View_GamesPerBuilding", "gamesPerBuilding");
			yield return CreateView("View_GamesBySystem", "gamesBySystem");
		}

		if (convention.HasProgramModule && convention.HasGameModule)
			yield return CreateView("View_ProgramAndGames", "programAndGames");
	}

	private ViewOption CreateView(string labelKey, string path)
		=> new(Strings.Get(labelKey), $"event/{EventId}/{path}");

	private sealed record ViewOption(string Label, string Href);
}
