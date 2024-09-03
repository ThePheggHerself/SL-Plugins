using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.NonAllocLINQ;

namespace CustomCommands.Commands.Misc
{
    public class MassNaming
    {
        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class RenameAll : ICustomCommand
        {
            public string Command => "renameall";

            public string[] Aliases { get; } = { };

            public string Description => "Sets every players' name based on an input. Use prefix \"h\" for help, or just \"A\" for everyone";

            public string[] Usage { get; } = { "Prefix", "Nickname" };

            public PlayerPermissions? Permission => PlayerPermissions.PlayersManagement;
            public string PermissionString => string.Empty;

            public bool RequirePlayerSender => false;

            public bool SanitizeResponse => false;

            internal static class help
            {
                internal const string general = "This command is an advanced(tm) tool to rename everyone in the server in one swoop.\n" +
                                                "   - Prefixes: Used to direct the command in a certain way\n" +
                                                "   - Nickname: A special (or plain text) string of the desired new nicknames";

                internal const string unname = "Use the command \"unnameall\" to reset everyone back to their normal names";

                internal const string prefixes = "Main prefixes: (This will make more sense if I add the scuffed ones back)\n" +
                                                "   - A: Renames everyone based on the provided string\n" +
                                                //"   - Rx,y,z,...: Renames everyone matching a desired role(s)\n" +
                                                //"   - Tx,y,z,...: Renames everyone matching a desired team(s)\n" +
                                                //"   - Nx: Renames only the specified number of players (replace x with number, e.g. \"N5\"\n" +
                                                "   - h: Shows this text!";// Or if used with a prefix, shows the available options (e.g. \"hT\")\n";// +
                                                //"Sub-prefixes:\n" +
                                                //"(These are 'modifiers' so to speak of the main prefixes)\n" +
                                                //"   - u: Only renames people without a nickname already (use before main prefix, e.g. \"uA\")\n" +
                                                //"   - a: Proccesses players sorted alphabetically ascending (use before main prefix, e.g. \"aA\")\n" +
                                                //"   - d: Proccesses players sorted alphabetically descending\n" +
                                                //"   - !: Renames people based on the opposite of the criteria (use before main prefix, e.g. \"!T0\", \"!uA\")\n";// +
                                                                                                                                                                 //"   - &: Combines multiple criteria/conditions (e.g. \"N5&T4\")";

                internal const string parsing = "There are a few special tags that get replaced which you can use in the nickname string:\n" +
                                                "   - {b}: Gets replaced with the player's default name\n" +
                                                //"      - {b:i:j}: The players name but with a certain number of characters chopped off ('i' from the start, and 'j' from the end)\n" +
                                                //"      - {b:v+i:v+j}: The players name but with all the characters before/after the first/last vowel chopped off, plus an optional extra i/j characters\n" +
                                                "   - {n}: A number that counts up for each player renamed\n" +
                                                "   - {a}: A letter of the alphabet that increases for each player renamed (gets funky after 26)\n" +
                                                //"   - {rnd:x:y}: A random integer between x (inclusive) and y (exclusive)\n" +
                                                //"   - {rndu:x:y}: A random unique (no repeats) integer between x (inclusive) and y (exclusive)\n" +
                                                "   - {r}: The player's role\n" +
                                                "   - {C}: The player's class";
                private static string _rHelp()
                {
                    string s = "Available roles (use the number):\n";
                    foreach (var c in Enum.GetValues(typeof(RoleTypeId)))
                        s += ((byte)c).ToString() + " : " + c.ToString() + "\n";
                    return s;
                }
                internal static string cHelp => _rHelp();

                private static string _tHelp()
                {
                    string s = "Available teams (use the number):\n";
                    foreach (var t in Enum.GetValues(typeof(Team)))
                        s += ((byte)t).ToString() + " : " + t.ToString() + "\n";
                    return s;
                }
                internal static string tHelp => _tHelp();
            }

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                try
                {
                    string prefix = arguments.FirstOrDefault();
                    if (prefix.StartsWith("h"))
                    {
                        //switch (prefix.Last())
                        //{
                        //    case 'C':
                        //        response = help.cHelp; return true;
                        //    case 'T':
                        //        response = help.tHelp; return true;
                        //}

                        response = $"<b>Welcome to mass renaming 101!</b>\n\n{help.general}\n\n{help.unname}\n\n{help.prefixes}\n\n{help.parsing}";
                        return true;
                    }


                    if (!sender.CanRun(this, arguments, out response, out var players, out _))
                        return false;

                    players = Player.GetPlayers();

                    bool prfxA = prefix.Contains('A');
                    //bool prfxN = prefix.Contains('N');
                    //bool prfxR = prefix.Contains('R');
                    //bool prfxT = prefix.Contains('T');
                    //bool prfxU = prefix.Contains('u');
                    if (!(prfxA))// || prfxN || prfxR || prfxT))
                    {
                        response = "Invalid prefix";
                        return false;
                    }

                    //bool prfxAsc = prefix.Contains('a');
                    //bool prfxDesc = prefix.Contains('d');
                    //bool prfxNOT = prefix.Contains('!');


                    Func<char, string, List<int>> f = (c, x) =>
                    {
                        bool cNum = false;
                        int temp = 0;
                        List<int> ret = new List<int>();
                        foreach (var y in x)
                        {
                            if (y == c)
                            {
                                cNum = true;
                                continue;
                            }
                            if ('0' <= y && y <= '9' && cNum == true)
                                temp = temp * 10 + (y - '0');
                            else if (y == ',' && cNum == true)
                            {
                                ret.Add(temp);
                                temp = 0;
                                continue;
                            }
                            else break;
                        }
                        return ret;
                    };

                    int n = Player.Count;
                    //if (prfxN)
                    //{
                    //    n = f('N', prefix).FirstOrDefault();
                    //    if (prfxNOT)
                    //        n = Player.Count - n;
                    //}

                    //var roles = f('R', prefix).Select(y => (RoleTypeId)y);
                    //var teams = f('T', prefix).Select(y => (Team)y);

                    //if (prfxAsc || prfxDesc)
                    //{
                    //    players.Sort(Comparer<Player>.Create((x, y) => x.Nickname.CompareTo(y.Nickname)));

                    //    if (prfxDesc)
                    //        players.Reverse();
                    //}
                    /*else*/ players.ShuffleList();

                    string nick = string.Join(" ", arguments.Skip(1));

                    int i = 0;
                    while (i < n && i < players.Count)
                    {
                        var plr = players.ElementAt(i);
                        //if (!(prfxNOT != (
                        //    prfxA ||
                        //    prfxR && roles.Contains(plr.Role) ||
                        //   prfxT && teams.Contains(plr.Team) ||
                        //    prfxU && plr.DisplayNickname == ""
                        //    ))) continue;

                        StringBuilder temp = new StringBuilder(nick);
                        temp.Replace("{b}", plr.Nickname);
                        temp.Replace("{n}", (i + 1).ToString());
                        temp.Replace("{a}", ((char)(i + 'A')).ToString());
                        temp.Replace("{t}", plr.Team.ToString());
                        temp.Replace("{r}", plr.Role.ToString());

                        plr.DisplayNickname = temp.ToString();
                        i++;
                    }

                    response = $"Renamed {n} {(n != 1 ? "people" : "person")}";

                    return true;
                } catch (Exception e)
                {
                    response = "You borked something\n" + e.Message;
                    return false;
                }
            }

            [CommandHandler(typeof(RemoteAdminCommandHandler))]
            public class UnnameAll : ICustomCommand
            {
                public string Command => "unnameall";

                public string[] Aliases { get; } = { };

                public string Description => "Resets every players' nickname";

                public string[] Usage { get; } = { };

                public PlayerPermissions? Permission => PlayerPermissions.PlayersManagement;
                public string PermissionString => string.Empty;

                public bool RequirePlayerSender => false;

                public bool SanitizeResponse => false;

                public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
                {
                    if (!sender.CanRun(this, arguments, out response, out var players, out _))
                        return false;

                    players = Player.GetPlayers();

                    int c = 0;
                    foreach (Player plr in players)
                    {
                        plr.DisplayNickname = "";
                        c++;
                    }

                    response = $"Reset the nicknames of {c} players";

                    return true;
                }


            }
        }
    }
}
