using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace eso_bot
{
    class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        string token = "";
        char prefix;

        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        public async Task Start()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            //get variables from text file
            try
            {
                StreamReader sr = new StreamReader("config.txt");
                token = sr.ReadLine();
                token = token.Replace(" ", "");
                token = token.Replace("token=", "");
                string sprefix = sr.ReadLine();
                sprefix = sprefix.Replace(" ", "");
                sprefix = sprefix.Replace("prefix=", "");
                prefix = sprefix.ToCharArray().First();
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{DateTime.Now,0:t} [Exception] {e.Message}");
            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            client.Log += Log;

            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }


        public async Task HandleCommand(SocketMessage msgParam)
        {
            var msg = msgParam as SocketUserMessage;
            if (msg == null) return;
            int argPos = 0;

            if (!(msg.HasCharPrefix(prefix, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            var context = new CommandContext(client, msg);
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
                //Console.WriteLine($"{DateTime.Now,0:t} [{context.Guild.Name,8}] {result.ErrorReason}");
        }


        private Task Log (LogMessage msg)
        {
            Console.WriteLine($"{DateTime.Now,0:t} [{msg.Severity,8}] {msg.Message}");
            return Task.CompletedTask;
        }
    }
}
