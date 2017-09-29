using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;
using Discord.Commands;

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
    }
}
