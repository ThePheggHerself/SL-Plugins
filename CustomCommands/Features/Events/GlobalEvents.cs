using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Zones.Heavy;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Events
{
	public class GlobalEvents
	{

		[PluginEvent]
		public void RoundRestart(RoundRestartEvent ev)
		{
			Plugin.CurrentEvent = EventType.NONE;
		}

		[PluginEvent]
		public bool TeamRespawn(TeamRespawnEvent args)
		{
			if (Plugin.EventInProgress)
				return false;
			else return true;
		}

		[PluginEvent]
		public bool PlayerInteractDoor(PlayerInteractDoorEvent args)
		{
			if (Plugin.EventInProgress)
			{
				if (args.Door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
					return true;
				else return false;
			}
			return args.CanOpen;
		}

		[PluginEvent]
		public bool SCP914Activate(Scp914ActivateEvent args)
		{
			if (Plugin.EventInProgress)
				return false;
			else return true;
		}

		[PluginEvent]
		public bool PlayerInteractElevator(PlayerInteractElevatorEvent args)
		{
			if (Plugin.EventInProgress)
			{
				return false;
			}
			else
			{
				if (args.Player.TemporaryData.Contains("plock"))
				{
					return false;
				}

				return true;
			}
		}
	}
}
