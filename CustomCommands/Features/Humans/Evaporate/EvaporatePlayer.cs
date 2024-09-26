using Footprinting;
using PlayerStatsSystem;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Humans.Evaporate
{
    public class EvaporatePlayer
    {
        public static void Evaporate(List<Player> plrs, Player attacker = null) { 
            foreach (var plr in plrs)
            {
                Evaporate(plr, attacker);
            }
        }
        public static void Evaporate(Player plr, Player attacker = null)
        {
            DisruptorDamageHandler temp = new DisruptorDamageHandler(new Footprint((attacker == null ? Server.Instance : attacker).ReferenceHub), 69420);
            plr.Damage(temp);
        }
    }
}
