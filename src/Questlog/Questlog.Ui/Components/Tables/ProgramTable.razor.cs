using Microsoft.AspNetCore.Components;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;

namespace Questlog.Ui.Components.Tables;

public partial class ProgramTable : EventTableBase<ProgramItem>
{
	[Inject]
	private IProgramService ProgramService { get; set; } = null!;

	protected override Task<IReadOnlyList<ProgramItem>> LoadAsync(string eventId)
		=> ProgramService.GetAllAsync(eventId);
}
