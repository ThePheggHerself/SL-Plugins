using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Events.Infection
{
	public class InfectionEvents
	{
		[PluginEvent, PluginPriority(LoadPriority.Highest)]
		public bool PlayerDying(PlayerDyingEvent args)
		{
			if (Plugin.CurrentEvent == EventType.Infection && args.DamageHandler is AttackerDamageHandler)
			{
				args.Player.ReferenceHub.roleManager.ServerSetRole(args.Attacker.Role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.AssignInventory);

				return false;
			}

			return true;
		}
	}
}
