using Interactables.Interobjects.DoorUtils;
using MEC;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Map.RollingBlackouts
{
	public class BlackoutEvents
	{
		[PluginEvent]
		public bool PlayerInteractDoor(PlayerInteractDoorEvent ev)
		{
			if (!Plugin.EventInProgress && RoomLightController.IsInDarkenedRoom(ev.Player.Position) && ev.Door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
				return false;

			return ev.CanOpen;
		}

		[PluginEvent]
		public void RoundStartEvent(RoundStartEvent ev)
		{
			BlackoutManager.DelayThisRound = UnityEngine.Random.Range(Plugin.Config.MinBlackoutTime, Plugin.Config.MaxBlackoutTime);
			BlackoutManager.TriggeredThisRound = false;
		}		
	}
}
