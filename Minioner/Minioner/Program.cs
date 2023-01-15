using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Minioner.Common;
using Minioner.Init;
using Minioner.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build();
var client2 = new DiscordSocketClient();
var commands = new CommandService(new CommandServiceConfig
{
    LogLevel = LogSeverity.Verbose,
});

// DI
Startup.Init();
Startup.RegisterInstance(client2);
Startup.RegisterInstance(commands);
Startup.RegisterType<ICommandHandler, CommandHandler>();
Startup.RegisterInstance(config);

// set global vars
Globals.GoogleToken = config.GetRequiredSection("Settings")["GoogleToken"];
Globals.GoogleSearchEngineID = config.GetRequiredSection("Settings")["GoogleSearchEngineID"];
Globals.NeutrinoAPIKey = config.GetRequiredSection("Settings")["NeutrinoAPIKey"];
Globals.NeutrinoRoute = config.GetRequiredSection("Settings")["NeutrinoRoute"];
var token = config.GetRequiredSection("Settings")["DiscordBotToken"];

// startup program. will run until stopped, waiting for commands
await MainAsync();

async Task MainAsync()
{
    // sstartup command handler
    await Startup.ServiceProvider.GetRequiredService<ICommandHandler>().InstallCommandsAsync();
    await client2.LoginAsync(TokenType.Bot, token);
    await client2.StartAsync();
    await Task.Delay(Timeout.Infinite);
}