using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Questlog.Core.Interfaces;
using Questlog.Core.Models;
using Questlog.Ui.Tests.Components.Tables;

namespace Questlog.Ui.Tests.Pages.Viewers;

public abstract class ViewerPageContext : TableContext
{
	protected ViewerPageContext()
	{
		var convention = new Convention { Id = "c1", Name = "Alpha Con", Logo = "/media/alpha.png" };

		ConventionService.GetAsync("c1", Arg.Any<CancellationToken>()).Returns(Task.FromResult<Convention?>(convention));
		ConventionService.GetAsync("missing", Arg.Any<CancellationToken>()).Returns(Task.FromResult<Convention?>(null));
		Services.AddSingleton(ConventionService);
	}

	protected IConventionService ConventionService { get; } = Substitute.For<IConventionService>();
}
