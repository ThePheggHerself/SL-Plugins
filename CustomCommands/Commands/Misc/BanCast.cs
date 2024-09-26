using CommandSystem;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCommands.Commands.Misc
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class BanCast : ICustomCommand
    {
        public string Command => "bancast";

        public string[] Aliases { get; } = { "bcast" };

        public string Description => "Shoots a ray that bans the person you're looking at";

        public string[] Usage { get; } = { "Duration" };

        public PlayerPermissions? Permission => PlayerPermissions.LongTermBanning;

        public string PermissionString => string.Empty;

        public bool RequirePlayerSender => false;

        public bool SanitizeResponse => false;

        private static TimeSpan GetBanDuration(char unit, int amount)
        {
            switch (unit)
            {
                default:
                    return new TimeSpan(0, 0, amount, 0);
                case 'h':
                    return new TimeSpan(0, amount, 0, 0);
                case 'd':
                    return new TimeSpan(amount, 0, 0, 0);
                case 'w':
                    return new TimeSpan(7 * amount, 0, 0, 0);
                case 'M':
                    return new TimeSpan(30 * amount, 0, 0, 0);
                case 'y':
                    return new TimeSpan(365 * amount, 0, 0, 0);
            }
        }


        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                long duration = 0;
                if (arguments.Any())
                {
                    string thing = arguments.First();
                    char unit = thing.Last();
                    if (int.TryParse(thing.Where(x => x >= '0' && x <= '9').ToString(), out int num))
                        duration = GetBanDuration(unit, num).Ticks;
                }

                Player sndr = Player.Get(sender);
                Player victim = null;
                var cam = sndr.Camera.transform;
                var ray = new Ray(cam.position, cam.rotation.eulerAngles);
                RaycastHit[] hits = Physics.RaycastAll(ray, 5000f, 2);
                foreach (var hit in hits)
                {
                    if (Player.TryGet(hit.rigidbody.gameObject, out var plr))
                    {
                        if (plr == sndr)
                            continue;
                        else
                        {
                            victim = plr;
                            break;
                        }
                    }
                }

                if (victim != null)
                {
                    victim.Ban("bang", duration);
                    response = $"Banned {victim.Nickname}";
                }
                else response = "You missed!";
                return true;
            }
            catch (Exception ex)
            {
                response = ex.Message;
                return false;
            }
        }
    }
}

