# ESO_bot
open source raid bot 
ESO Bot by Keiaos Documentation (or at least an attempt)
Due to somewhat popular demand, here is an “open source” version of the raid bot I created for ETC. It doesn’t do anything too fancy other than handle raid signups. There are no channel restrictions because I wanted to make this as simple as possible but anyone is welcome to edit the code to fit their needs. This bot uses text files to hold data because it does not require a database (yay!) and to make editing easier for those in control of the sign up process. The files are found inside the debug folder of the bot, config.txt and defaults.txt are in Debug and the files generated from signups are inside the raids folder inside the Debug folder. IF YOU MOVE OR RENAME THESE FILES, you must go into the code and change their file paths in the commands and bot initialization to ensure that no errors occur.

I don’t think there will be any updates to this bot unless I have literally nothing to do. At the rate that discord updates their API there is a possibility for this program to stop working at some point in the future. Updates can be as simple as opening up visual studio and updating the .Net packages or as complicated as rewriting the majority of the code (it has happened to me several times before), so I suggest if you know what you’re doing (or even don’t but are willing to take the time to learn how) that you regularly update the packages, and possibly rework the code when necessary. Luckily whenever I have had to rework my code it has been simple enough that the commands will all stay the same, it just requires a different method of logging your bot in.

With that being said, this version of the bot uses Discord.Net v1.0.2, below you’ll find how to set up the bot and get it running, as well as command documentation giving a brief explanation of the commands and how they work.

Set up:
•	Create bot through discord dev page and get token
•	Add your bot to your server
•	Before starting bot, token and prefix must be defined in config.txt file inside \eso_bot\eso_bot\bin\Debug
•	Place token on same line as “token =”
  o	Example: token = your token here
•	Same with prefix, prefix will be first char after “prefix =”
  o	Example: prefix = !
    	Prefix will be !
  o	Example: prefix = !?
    	Prefix will be !

Admin commands: (must have admin access in server to use these commands)
•Openraid (aliases: “open”, “or”)
  o	Parameters: 
    	Raid: name of the text file to be created, one word for simplicity, parameter must be one word, examples: Maw, MOL, Maw_of_Lorkhaj. Keep in mind players must include raid name when signing up
  o	Creates text file in “\eso_bot\eso_bot\bin\Debug\raids\” with the name given with the command
  o	Command examples
    	!openraid maw
      •	Generates maw.txt
    	!or hofHM
      •	Generates hofHM.txt
    	!open weekly
      •	Generates weekly.txt
•Closeraid (aliases: “clearraid”, “clear”, “close”, “cr”)
  o	Parameters: 
    	Raid: name of the text file to be deleted. Not case sensitive, if Maw.txt exists and command !clear MAW is called, file will be deleted
  o	Deletes file with given name in raids folder if file exists
  o	Command examples:
    	!clearraid maw
    	!close hof
    	!cr That_one_raid_with_a_lot_of_words
      •	Naming extremely unconventional but will still work because the raid name is one word

Sign up Commands:
•Signup (alias: “su”)
  o	Parameters: All are non-case sensitive
    	Raid: name of raid to signup for, name of text file in raids folder, if raid doesn’t exist in folder, user not signed up. CANNOT BE LEFT BLANK
    	Roles (Optional): roles user wants to sign up for raid, available choices: dps, damage, tank, heal, heals, or healer. If roles are left out, user defaults are used
  o	Signs up user for specified raid if available with specified roles or default roles
  o	Command examples:
    	!su hrc dps heals tank
      •	Signs up user in hrc.txt as dps heals and tank
    	!signup dps
      •	Error: bot sees dps as the raid name not roles, raid name cannot be omitted
    	!signup maw
      •	Signs user up in maw.txt as default roles, if user has not specified defaults, dps is used as the default
•Withdraw
  o	Parameters:
    	Raid: name of raid to withdraw from, name of text file in raids folder, if raid doesn’t exist in folder, user not withdrawn
  o	Removes user from specified raid signup
  o	Command examples:
    	!withdraw maw
      •	Withdraws user from maw.txt
    	!withdraw
      •	Error: bot will say raid cannot be left blank
•Status
  o	Parameters:
    	Raid: Name of raid to get status for
  o	Displays list of users signed up with roles, role counts, and count of users signed up
  o	Command examples:
    	!status hof
      •	If hof.txt exists, displays status of signups. If no users signed up, sends message that signups are empty
    	!status
      •	Error: must include name of raid to check status for
•Raidlist (alias: “list”)
  o	Reads the raids folder and lists all available text files
  o	KEEP THIS FOLDER EMPTY OF NON RAID FILES, this folder was made specifically for holding files for raid signups, if non raid text files are present they can be overwritten and even deleted
  o	Command examples
    	!list
      •	Reads raids folder and lists available text files
•Default
  o	Parameters:
    	Roles: roles to be added to defaults file for use of default roles in signups
    	Not case sensitive, can contain spaces
  o	Defaults are used when roles are omitted in signup process, optional
  o	Command examples
    	!default tank
      •	Writes default as tank in defaults.txt
    	!deault dps heals tank
      •	Writes default as dps heals tank
    	!default spicy dongers
      •	Tried to make this as troll proof as possible, any “roles” other than the three standard roles will be marked as dps in defaults
      •	Default will be dps

General Commands:
•Help
  o	Parameters:
    	Command (optional): specific command to get help for
  o	If command parameter not given, bot lists all available commands
  o	If command parameter is given, bot will display help for specified command
  o	Command examples:
    	!help
      •	Lists all commands for the bot
    	!help signup
      •	Lists help for signup command
    	!help su
  	  •	Lists help for signup command
