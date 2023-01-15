using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Minioner.Common;
using Minioner.Init;

namespace Minioner.Services;

public class CommandHandler : ICommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    public CommandHandler(DiscordSocketClient client,CommandService commands)
    {
        _client = client;
        _commands = commands;
    }

    public async Task InstallCommandsAsync()
    {
        _client.MessageReceived += HandleCommandAsync;
        await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), Startup.ServiceProvider);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Bail out if it's a System Message.
        if (messageParam is not SocketUserMessage msg) 
            return;

        // We don't want the bot to respond to itself or other bots.
        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) 
            return;

        // Create a Command Context.
        var context = new SocketCommandContext(_client, msg);
        
        var markPos = 0;
        if (msg.HasCharPrefix('!', ref markPos) || msg.HasCharPrefix('?', ref markPos))
        {
            var result = await _commands.ExecuteAsync(context, markPos, Startup.ServiceProvider);
        }
    }
}