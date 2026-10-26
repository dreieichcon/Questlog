using Questlog.Components;
using Questlog.Configuration;
using Questlog.Core.Interfaces;
using Questlog.Ui;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddQuestlog(QuestlogSettings.FromConfiguration(builder.Configuration));

var app = builder.Build();

// start the timers and the display sync, so both see every tick
_ = app.Services.GetRequiredService<ITimerService>();
_ = app.Services.GetRequiredService<IDisplaySyncService>();

app.UseAntiforgery();
app.MapStaticAssets();
app.MapGet("/health", () => Results.Ok());

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddAdditionalAssemblies(typeof(UiAssembly).Assembly);

await app.RunAsync();
