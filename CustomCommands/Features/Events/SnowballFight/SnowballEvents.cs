using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Events.SnowballFight
{
	public class SnowballEvents
	{
		[PluginEvent]
		public void PlayerDeath(PlayerDeathEvent args)
		{
			if (Plugin.CurrentEvent == EventType.SnowballFight)
			{
				args.Attacker.Heal(15);
			}
		}
	}
}
