using CommandSystem;
using CustomCommands.Features.Humans.Evaporate;
using PluginAPI.Core;
using System;
using Utils;

namespace CustomCommands.Commands.Grenade
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class EvaporateCommand : ICustomCommand
    {
        public string Command => "evaporate";

        public string[] Aliases => new string[] { "pickupthatcan" };

        public string Description => "Causes the player to evaporate";

        public string[] Usage { get; } = { "%player%" };

        public PlayerPermissions? Permission => null;
        public string PermissionString => "cuscom.grenade";

        public bool RequirePlayerSender => false;

        public bool SanitizeResponse => false;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CanRun(this, arguments, out response, out var players, out _))
                return false;

            foreach (Player plr in players)
            {
                if (plr.Role == PlayerRoles.RoleTypeId.Spectator || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
                    continue;
                EvaporatePlayer.Evaporate(plr);
            }
            response = "Player successfully evaporated";
            return true;
        }
    }
}