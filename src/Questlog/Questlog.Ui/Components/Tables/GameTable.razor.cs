using Microsoft.AspNetCore.Components;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Ui.Components.Tables;

public partial class GameTable : EventTableBase<Game>
{
	[Inject]
	private IGameService GameService { get; set; } = null!;

	protected override Task<IReadOnlyList<Game>> LoadAsync(string eventId)
		=> GameService.GetAllAsync(eventId);
}
