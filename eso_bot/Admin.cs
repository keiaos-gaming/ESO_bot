using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace eso_bot
{
    public class Admin : ModuleBase
    {
        [Command("openraid")]
        [Alias("open", "or")]
        [Summary("Creates text file to hold names and roles for sign ups")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task OpenRaidCmd([Summary("Name for text file / name of raid, one word only.")] string raid)
        {
            string fileName = raid + ".txt";
            fileName = Path.GetFullPath(fileName).Replace(fileName, "");
            fileName = fileName + @"\raids\" + raid + ".txt";
            if (File.Exists(fileName))
            {
                await ReplyAsync($"File Name {fileName} already exists, use another name or clear the raid.");
            }
            else
            {
                File.Create(fileName).Close();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                await ReplyAsync($"Raid signups now open for {raid}!");               
            }
        }

        [Command("closeraid")]
        [Alias("clearraid", "clear", "close", "cr")]
        [Summary("Deletes signups for specified raid.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CloseRaidCmd([Remainder, Summary("Name of the text file to be deleted.")] string name = null)
        {
            string sendmsg = "";
            
            if (name == null)
            {
                sendmsg = "Please enter the file name with the command.";
            }
            else
            {
                try
                {

                    string fileName = name + ".txt";
                    fileName = Path.GetFullPath(fileName).Replace(fileName, "");
                    fileName = fileName + @"\raids\" + name + ".txt";
                    if (!File.Exists(fileName))
                    {
                        sendmsg = "Error: " + fileName + " does not exist.";
                    }
                    else
                    {
                        File.Delete(fileName);
                        sendmsg = "Deleted " + name + ".txt successfully.";
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
            }

            await ReplyAsync(sendmsg);
        }
        
        [Command("rollcall")]
        [Alias("rc")]
        [Summary("Mentions users signed up for specified raid with a message that raid is forming.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task RollCallCmd([Summary("Name of raid to call roll call for.")] string raid = null)
        {
            if (raid == null)
            {
                await ReplyAsync("Please enter the raid name with the command.");
            }
            else
            {
                string fileName = raid + ".txt";
                fileName = Path.GetFullPath(fileName).Replace(fileName, "");
                fileName = fileName + @"\raids\" + raid + ".txt";
                if (!File.Exists(fileName))
                {
                    await ReplyAsync("Error: " + fileName + " does not exist.");
                }
                else
                {
                    string line = "";
                    string sendmsg = "";
                    try
                    {
                        StreamReader sr = new StreamReader(fileName);
                        line = sr.ReadLine();
                        if (line == null)
                        {
                            sendmsg = "No users signed up for " + raid + " raid.";
                        }
                        else
                        {
                            var guild = Context.Guild as SocketGuild;
                            var users = guild.Users;
                            int count = 0;
                            while (line != null && count <= 11)
                            {
                                SocketUser player = null;
                                try
                                {
                                    player = users.Where(x => x.Username == line).First() as SocketUser;
                                }
                                catch(Exception e)
                                {
                                    Console.WriteLine($"Player {line} in {raid}.txt not found in server.");
                                }
                                if (player != null)
                                {
                                    sendmsg = sendmsg  + player.Mention + " ";
                                    count++;
                                }
                                line = sr.ReadLine();
                                line = sr.ReadLine();
                            }
                            sendmsg = sendmsg + "forming up for " + raid + "! Time to log in.";
                        }
                        await ReplyAsync(sendmsg);
                        sr.Close();
                    }
                    catch (Exception e)
                    {
                        await ReplyAsync("Exception: " + e.Message);
                    }
                }
            }
        }
    }
}
