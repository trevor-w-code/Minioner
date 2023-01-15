using Discord;
using Discord.Commands;
using Google.Apis.Services;
using Minioner.Common;
using Minioner.Services;
using Newtonsoft.Json;

namespace Minioner.Modules;

public class Commands : ModuleBase<SocketCommandContext>
{
    public CommandService CommandService { get; set; }

    [Command("halp")]
    [Summary("Help command.")]
    public Task Halp()
    {
        return ReplyAsync("Send a '!minion' command followed by a search modifier.\nExample: !minion wholesome");
    }

    [Command("minion")]
    public async Task MinionTime([Remainder][Summary("Modifer text")] string modifier = "")
    {
        await Context.Message.ReplyAsync($"It's Minion time, {Context.User.Username}! Standby for a sick meme...");
        var random = new Random();
        if (modifier == "")
        {
            modifier = DefaultSearchModifiers.Modifiers[random.Next(DefaultSearchModifiers.Modifiers.Length - 1)];
        }
        else
        {
            // TODO add to DI
            var filter = new BadWordFilter();
            // check input for bad words
            var response = await filter.CheckInput(modifier);
            // convert reponse string to obj (keyvalue)
            dynamic apiResponse = JsonConvert.DeserializeObject(response);
            if (apiResponse["is-bad"] == "true")
            {
                modifier = DefaultSearchModifiers.Modifiers[random.Next(DefaultSearchModifiers.Modifiers.Length - 1)];
                await Context.Message.ReplyAsync($"Smh {Context.User.Username}...Let's try a {modifier} meme instead");
            }
        }
        // setup google service
        var svc = new Google.Apis.Customsearch.v1.CustomsearchService(new BaseClientService.Initializer { ApiKey = Globals.GoogleToken });     
        var searchTerm = modifier + " " + "minion meme";
        // call google custom search
        var result = GoogleService.List(svc, searchTerm);
        // get random result from results
        var img = result.Items[random.Next(9)].Link;
        await Context.Message.ReplyAsync($"{img}");
    }
}