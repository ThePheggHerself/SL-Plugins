using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomCommands.Commands.Misc
{
    public class MassNaming
    {
        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class RenameAll : ICustomCommand
        {
            public string Command => "renameall";

            public string[] Aliases { get; } = { };

            public string Description => "Sets every players' name based on an input. Use prefix \"h\" for help, or just \"a\" for everyone";

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

                internal const string prefixes = "Main prefixes:\n" +
                                                "   - A: Renames everyone based on the provided string\n" +
                                                "   - Rx,y,z,...: Renames everyone matching a desired role(s)\n" +
                                                "   - Tx,y,z,...: Renames everyone matching a desired team(s)\n" +
                                                "   - Nx: Renames only the specified number of players (replace x with number, e.g. \"N5\"\n" +
                                                "   - h: Shows this text! Or if used with a prefix, shows the available options (e.g. \"hT\")\n" +
                                                "Sub-prefixes:\n" +
                                                "(These are 'modifiers' so to speak of the main prefixes)\n" +
                                                "   - u: Only renames people without a nickname already (use before main prefix, e.g. \"uA\")\n" +
                                                "   - a: Proccesses players sorted alphabetically ascending (use before main prefix, e.g. \"aA\")\n" +
                                                "   - d: Proccesses players sorted alphabetically descending\n" +
                                                "   - !: Renames people based on the opposite of the criteria (use before main prefix, e.g. \"!T0\", \"!uA\")\n";// +
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
                        s += c.ToString() + " : " + ((RoleTypeId)c).ToString() + "\n";
                    return s;
                }
                internal static string cHelp => _rHelp();

                private static string _tHelp()
                {
                    string s = "Available teams (use the number):\n";
                    foreach (var t in Enum.GetValues(typeof(Team)))
                        s += t.ToString() + " : " + ((Team)t).ToString() + "\n";
                    return s;
                }
                internal static string tHelp => _tHelp();
            }

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                string prefix = arguments.FirstOrDefault();
                if (prefix.Equals("h"))
                {
                    response = $"<b>Welcome to mass renaming 101!</b>\n\n{help.general}\n\n{help.unname}\n\n{help.prefixes}\n\n{help.parsing}";
                    return true;
                }
                else if (prefix.StartsWith("h") && prefix.Length == 2)
                    switch (prefix[1])
                    {
                        case 'c':
                            response = help.cHelp; return true;
                        case 't':
                            response = help.tHelp; return true;
                    }

                if (!sender.CanRun(this, arguments, out response, out var players, out _))
                    return false;

                players = Player.GetPlayers();

                bool prfxA = prefix.Contains('A');
                bool prfxN = prefix.Contains('N');
                bool prfxR = prefix.Contains('R');
                bool prfxT = prefix.Contains('T');
                bool prfxU = prefix.Contains('u');
                bool prfxAsc = prefix.Contains('a');
                bool prfxDesc = prefix.Contains('d');
                bool prfxNOT = prefix.Contains('!');

                bool testNum = true;
                Func<char, char, bool> f = (c, x) =>
                {
                    if (x.Equals(c) && testNum == true)
                    {
                        testNum = true;
                        return false;
                    }
                    else if (('0' <= x && x <= '9' || x == ',') && testNum == true) return true;
                    else testNum = false;
                    return false;
                };

                int n = Player.Count;
                if (prfxN)
                {
                    testNum = true;
                    n = int.Parse(prefix.Where(x => f('N', x)).ToString());
                    if (prfxNOT)
                        n = Player.Count - n;
                }

                testNum = true;
                var roles = prefix.Where(x => f('R', x)).ToString().Split(',').Select(y => (RoleTypeId)int.Parse(y));
                testNum = true;
                var teams = prefix.Where(x => f('T', x)).ToString().Split(',').Select(y => (Team)int.Parse(y));

                if (prfxAsc || prfxDesc)
                {
                    players.Sort(Comparer<Player>.Create((x, y) => x.Nickname.CompareTo(y.Nickname)));

                    if (prfxDesc)
                        players.Reverse();
                }
                else players.ShuffleList();

                string nick = string.Join(" ", arguments.Skip(1));

                int i = 0;
                while (i < n && i < players.Count)
                {
                    var plr = players[i];
                    if (!(prfxNOT != (
                        prfxA ||
                        prfxR && roles.Contains(plr.Role) ||
                        prfxT && teams.Contains(plr.Team) ||
                        prfxU && plr.DisplayNickname != ""
                        ))) continue;

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
