using Questlog.Configuration;
using Questlog.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddQuestlog(QuestlogSettings.FromConfiguration(builder.Configuration));

var app = builder.Build();

// start the timers
_ = app.Services.GetRequiredService<ITimerService>();

await app.RunAsync();
