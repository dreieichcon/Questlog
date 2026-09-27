using Conservices.Screen.Components;
using Conservices.Screen.Interfaces.Conservices;
using Conservices.Screen.Interfaces.Display;
using Conservices.Screen.Interfaces.Repositories;
using Conservices.Screen.Interfaces.Timers;
using Conservices.Screen.Repositories.Core;
using Conservices.Screen.Repositories.Sun;
using Conservices.Screen.Services.Conservices;
using Conservices.Screen.Services.Display;
using Conservices.Screen.Services.Timers;
using Conservices.Screen.Ui.Pages;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<ITimerService, TimerService>();
builder.Services.AddSingleton<IDisplaySyncService, DisplaySyncService>();

builder.Services.AddSingleton<ISunRepository, SunRepository>();
builder.Services.AddSingleton<IConventionRepository, ConventionRepository>();
builder.Services.AddSingleton<IConventionService, ConventionService>();

builder.Services.AddSingleton<IProgramRepository, ProgramRepository>();
builder.Services.AddSingleton<IProgramService, ProgramService>();

builder.Services.AddSingleton<IGameRepository, GameRepository>();
builder.Services.AddSingleton<IGameService, GameService>();

var app = builder.Build();

// initialize Timer
_ = app.Services.GetRequiredService<ITimerService>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);

	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
else
{
	//app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddAdditionalAssemblies(typeof(Home).Assembly);

await app.RunAsync();