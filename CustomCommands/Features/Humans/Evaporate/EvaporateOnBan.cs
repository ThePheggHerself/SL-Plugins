using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Humans.Evaporate
{
    public class EvaporateOnBan
    {
        [PluginEvent]
        public void OnPlayerBanned(PlayerBannedEvent args)
        {
            if (Player.TryGet(args.Player.UserId, out var plr) && plr.IsAlive)
                EvaporatePlayer.Evaporate(plr);
        }
        [PluginEvent]
        public void OnPlayerKicked(PlayerKickedEvent args)
        {
            if (Player.TryGet(args.Player.UserId, out var plr) && plr.IsAlive)
                EvaporatePlayer.Evaporate(plr);
        }
    }
}
