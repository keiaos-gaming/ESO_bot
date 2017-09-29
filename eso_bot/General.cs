using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord.Commands;
using Discord;
using Discord.WebSocket;

namespace eso_bot
{
    public class General : ModuleBase
    {
        private CommandService _service;

        public General(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("Shows what a specific command does and what parameters it takes.")]
        public async Task HelpAsync([Remainder, Summary("command to retrieve help for")] string command = null)
        {

            string prefix = "";
            string line;
            try
            {
                StreamReader sr = new StreamReader("config.txt");
                line = sr.ReadLine();
                prefix = sr.ReadLine();
                prefix = prefix.Replace(" ", "");
                prefix = prefix.Replace("prefix=", "");
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now,0:t} [Exception] {e.Message}");
            }

            
            if (command == null)
            {
                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description = $"**Command Help for {Context.Client.CurrentUser.Username}**"
                };

                foreach (var module in _service.Modules) //loop through modules from _service
                {
                    string description = null;
                    foreach (var cmd in module.Commands) //loop through commands in modules
                    {
                        var result = await cmd.CheckPreconditionsAsync(Context);
                        if (result.IsSuccess)
                            description += $"{prefix}{cmd.Aliases.First()}\n"; //if command passes, first alias AKA command name is added
                    }

                    if (!string.IsNullOrWhiteSpace(description)) //if the module wasn't empty, add info to field
                    {
                        builder.AddField(x =>
                        {
                            x.Name = module.Name;
                            x.Value = description;
                            x.IsInline = false;
                        });
                    }
                }
                //send help msg in DM to user
                //var dmchannel = await Context.User.GetOrCreateDMChannelAsync();
                //await dmchannel.SendMessageAsync("", false, builder.Build());

                //or post in channel where used
                await ReplyAsync("", false, builder.Build());
            }
            else //user asks for help for specific command
            {
                var result = _service.Search(Context, command);

                if (!result.IsSuccess)//command not found in search
                {
                    await ReplyAsync($"Sorry, **{command}** command doesn't exist.");
                    return;
                }

                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description = $"**Help for command {prefix}{command}**\n\n**Aliases:** "
                };

                foreach (var match in result.Commands)
                {
                    var cmd = match.Command;

                    builder.AddField(x =>
                    {
                        x.Name = string.Join(", ", cmd.Aliases);
                        x.Value =
                            $"**Summary:** {cmd.Summary}\n" +
                            $"**Parameters:** {string.Join(", ", cmd.Parameters.Select(p => p.Name))}: {string.Join("", cmd.Parameters.Select(p => p.Summary))}\n";
                        x.IsInline = false;
                    });
                }
                await ReplyAsync("", false, builder.Build());
            }
        }
    }
}
