using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord.Commands;

namespace eso_bot
{
    public class SignUp : ModuleBase
    {
        int maxSignUps = 11;

        [Command("signup")]
        [Alias("su")]
        [Summary("Signs user up for specified raid with specified roles.")]
        public async Task SignUpCmd(string raid, [Remainder]string roles = null)
        {
            int signUps = 0;
            bool playerAllowed = true;
            string line = "", sendmsg = "";

            //define filepath
            string fileName = raid + ".txt";
            fileName = Path.GetFullPath(fileName).Replace(fileName, "");
            fileName = fileName + @"\raids\" + raid + ".txt";

            if (!File.Exists(fileName)) //file doesnt exist
            {
                sendmsg = ($"Raid for {raid} doesn't exist!");
            }
            else
            {
                try
                {
                    //check if player is already signed up
                    StreamReader sr = new StreamReader(fileName);
                    line = sr.ReadLine();

                    //loop through file
                    while (line != null)
                    {
                        if (Context.Message.Author.Username == line)
                        {
                            //if player is found, msg sends and skips sign up process
                            sendmsg = "Only one signup allowed per person. You have already signed up.";
                            playerAllowed = false;
                        }
                        signUps++;
                        line = sr.ReadLine();
                    }
                    sr.Close();
                    signUps = (signUps) / 2;
                }
                catch (Exception e)
                {
                    await ReplyAsync("Exception: " + e.Message);
                }

                if (playerAllowed) // player not found in signup
                {
                    if (roles == null) // roles omitted, use defaults
                    {
                        bool defaultFound = false;
                        try
                        {
                            //search defaults for user
                            StreamReader sr = new StreamReader("defaults.txt");
                            line = sr.ReadLine();
                            while (line != null)
                            {
                                if (Context.Message.Author.Username == line)
                                {
                                    //user found roles are saved
                                    defaultFound = true;
                                    roles = sr.ReadLine();
                                }
                                line = sr.ReadLine();
                            }

                            if (!defaultFound) // not found in defaults.txt, uses dps as default
                            {
                                roles = "dps ";
                            }
                            sr.Close();
                        }
                        catch (Exception e)
                        {
                            await ReplyAsync("Exception: " + e.Message);
                        }

                        //adds names to sign up file
                        try
                        {
                            StreamWriter sw = new StreamWriter(@fileName, true);

                            //Write a line of text
                            sw.WriteLine(Context.Message.Author.Username);
                            sw.WriteLine(roles);
                            //close the file
                            sw.Close();
                        }
                        catch (Exception e)
                        {
                            await ReplyAsync("Exception: " + e.Message);
                        }

                        sendmsg = Context.Message.Author.Username + " has signed up as " + roles;

                    }

                    //user gives roles
                    else
                    {
                        //format roles for writing
                        string updatedRoles = "";
                        if (roles.ToUpper().Contains("DPS") || roles.ToUpper().Contains("DAMAGE"))
                        {
                            updatedRoles += "dps ";
                        }
                        if (roles.ToUpper().Contains("HEALER") || roles.ToUpper().Contains("HEALS") || roles.ToUpper().Contains("HEAL"))
                        {
                            updatedRoles += "healer ";
                        }
                        if (roles.ToUpper().Contains("TANK"))
                        {
                            updatedRoles += "tank ";
                        }

                        //adds name and roles to file
                        try
                        {
                            StreamWriter sw = new StreamWriter(@fileName, true);

                            sw.WriteLine(Context.Message.Author.Username);
                            sw.WriteLine(updatedRoles);
                            sw.Close();
                        }
                        catch (Exception e)
                        {
                            await ReplyAsync("Exception: " + e.Message);
                        }

                        sendmsg = Context.Message.Author.Username + " has signed up as " + updatedRoles;

                    }

                    //if raid is full 
                    if (signUps >= maxSignUps)
                    {
                        sendmsg += "\nRaid is full! Signed up as overflow.";
                    }
                }
            }
            await ReplyAsync(sendmsg);
        }

        [Command("withdraw")]
        [Summary("Withdraws user from specified raid if signed up.")]
        public async Task WithdrawCmd([Summary("Raid to withdraw from")] string raid = null)
        {
            String line;
            string sendmsg = "";

            //define file path
            string fileName = raid + ".txt";
            fileName = Path.GetFullPath(fileName).Replace(fileName, "");
            fileName = fileName + @"\raids\" + raid + ".txt";

            int i = 0;
            List<string> names = new List<string>();
            List<string> roles = new List<string>();
            bool playerFound = false;

            if (raid != null && File.Exists(fileName)) //command is correct and file exists
            {
                //read sign up list to see if player has already registered
                try
                {
                    StreamReader sr = new StreamReader(fileName);
                    line = sr.ReadLine();

                    //loop through file
                    while (line != null)
                    {
                        if (Context.Message.Author.Username == line)
                        {
                            //if player is found, msg sends, skips saving name and roles for rewrite
                            sendmsg = Context.Message.Author.Username + " removed from " + raid + " signups.";
                            line = sr.ReadLine();
                            playerFound = true;
                        }
                        else
                        {
                            //if not user, adds lines to names and roles for rewrite
                            names.Add(line);
                            line = sr.ReadLine();
                            roles.Add(line);
                            i++;
                        }

                        line = sr.ReadLine();
                    }
                    sr.Close();

                    //rewrite names and roles to file
                    StreamWriter sw = new StreamWriter(fileName);
                    for (int x = 0; x < i; x++)
                    {
                        sw.WriteLine(names[x]);
                        sw.WriteLine(roles[x]);
                    }
                    sw.Close();
                }
                catch (Exception e)
                {
                    await ReplyAsync("Exception: " + e.Message);
                }
                if (!playerFound) //user not in file
                {
                    sendmsg = "Player not found in signup list.";
                }
            }
            else if (raid == null) //parameter not given
            {
                sendmsg = "Please include a raid with the command.";
            }
            else if (!File.Exists(fileName)) //file doesnt exist, raid with specified name for file doesnt exist
            {
                sendmsg = "Error: " + raid + " raid doesn't exist";
            }

            await ReplyAsync(sendmsg);
        }

        [Command("status")]
        [Summary("Lists players signed up for specified raid")]
        public async Task StatusCmd([Summary("Name of raid for status.")] string raid = null)
        {
            if (raid == null) //no parameter given
            {
                await ReplyAsync("Please include the name of the raid with the command.");
            }
            else
            {
                //define file path
                string fileName = raid + ".txt";
                fileName = Path.GetFullPath(fileName).Replace(fileName, "");
                fileName = fileName + @"\raids\" + raid + ".txt";
                if (!File.Exists(fileName))
                {
                    await ReplyAsync($"Raid for {raid} does not exist.");
                }
                else
                {
                    string sendmsg = "";
                    int number = 0;
                    int dps = 0, tanks = 0, heals = 0;
                    string line;
                    try
                    {
                        //read file, if not empty add names and roles to message
                        StreamReader sr = new StreamReader(fileName);
                        line = sr.ReadLine();
                        if (line != null)
                        {
                            //first name and roles
                            sendmsg = "**Sign Up List**\n";
                            number++;
                            sendmsg = sendmsg + line + ": ";
                            line = sr.ReadLine();
                            if (line.Contains("dps"))
                            {
                                sendmsg += "*dps* ";
                                dps++;
                            }
                            if (line.Contains("tank"))
                            {
                                sendmsg += "*tank* ";
                                tanks++;
                            }
                            if (line.Contains("healer"))
                            {
                                sendmsg += "*heals* ";
                                heals++;
                            }
                            line = sr.ReadLine();
                        }

                        while ((line != null)) //if more than one name in signups
                        {
                            sendmsg = sendmsg + System.Environment.NewLine + line + ": ";
                            line = sr.ReadLine();
                            if (line.Contains("dps"))
                            {
                                sendmsg += "*dps* ";
                                dps++;
                            }
                            if (line.Contains("tank"))
                            {
                                sendmsg += "*tank* ";
                                tanks++;
                            }
                            if (line.Contains("healer"))
                            {
                                sendmsg += "*heals* ";
                                heals++;
                            }
                            number++;
                            line = sr.ReadLine();
                        }

                        sr.Close();
                        sendmsg += System.Environment.NewLine + dps + " dps " + tanks + " tanks " + heals + "heals , " + number + "/11 signed up";
                        if (number == 0) //file is empty
                        {
                            sendmsg = "No players signed up for raid.";
                        }

                    }
                    catch (Exception e)
                    {
                        await ReplyAsync("Exception: " + e.Message);
                    }

                    await ReplyAsync(sendmsg);
                }

            }
        }

        [Command("raidlist")]
        [Alias("list")]
        [Summary("Lists raids availble for signups.")]
        public async Task RaidListCmd()
        {
            //define file path
            string path = Path.GetFullPath("config.txt").Replace("config.txt", @"\raids");
            string[] folder = Directory.GetFiles(path);
            string sendmsg = "Available raids: ";

            //loop through array and get names of files
            foreach (string file in folder)
            {
                string raid = file.Replace(".txt", "");
                raid = raid.Replace(path + "\\", "");
                sendmsg += raid + " ";
            }
            if (folder.Count() == 0)//no files in folder
            {
                sendmsg = "No raids available.";
            }

            await ReplyAsync(sendmsg);
        }

        [Command("default")]
        [Summary ("Sets default roles to be used for raids when roles not specified.")]
        public async Task DefaultCmd([Remainder, Summary("Roles for deafults.")]string roles = null)
        {
            if (roles ==null) // no parameter given with command
            {
                await ReplyAsync("Please include roles with the command.");
            }
            else
            {
                string line;
                string newRoles = "";
                int i = 0;
                List<string> names = new List<string>();
                List<string> defaults = new List<string>();
                bool playerFound = false;
                

                //process roles and format
                if (roles.ToUpper().Contains("DPS") || roles.ToUpper().Contains("DAMAGE"))
                {
                    newRoles = newRoles + "dps ";
                }
                if (roles.ToUpper().Contains("TANK"))
                {
                    newRoles = newRoles + "tank ";
                }
                if (roles.ToUpper().Contains("HEAL") || roles.ToUpper().Contains("HEALER") || roles.ToUpper().Contains("HEALS"))
                {
                    newRoles += " healer ";
                }
                if (newRoles == "") //roles given did not contain dps tank or heal
                {
                    newRoles = "dps ";
                }

                try
                {
                    //read defaults to see if default was already given
                    StreamReader sr = new StreamReader("defaults.txt");
                    line = sr.ReadLine();

                    while (line != null)
                    {
                        if (Context.Message.Author.Username == line)
                        {
                            //if player is found, msg sends, skips saving name and roles for rewrite
                            names.Add(line);
                            defaults.Add(newRoles);
                            line = sr.ReadLine();
                            playerFound = true;
                            i++;

                        }
                        else
                        {
                            //if not user, adds lines to names and roles for rewrite
                            names.Add(line);
                            line = sr.ReadLine();
                            defaults.Add(line);
                            i++;
                        }

                        line = sr.ReadLine();
                    }
                    sr.Close();

                    if (!playerFound) // user not already in defaults file
                    {
                        names.Add(Context.Message.Author.Username);
                        defaults.Add(newRoles);
                        i++;

                    }

                    //write names back into file
                    StreamWriter sw = new StreamWriter("defaults.txt");
                    for (int x = 0; x < i; x++)
                    {
                        sw.WriteLine(names[x]);
                        sw.WriteLine(defaults[x]);
                    }
                    sw.Close();
                    await ReplyAsync(Context.Message.Author.Username + " registered " + newRoles + "as default.");
                }
                catch (Exception e)
                {
                    await ReplyAsync("Exception: " + e.Message);
                }
            }
        }

    }
}
