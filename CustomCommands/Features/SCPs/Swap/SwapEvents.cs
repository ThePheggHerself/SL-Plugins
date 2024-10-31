using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.SCPs.Swap
{
	public class SwapEvents
	{
		[PluginEvent]
		public void PlayerSpawn(PlayerSpawnEvent args)
		{
			if (args.Player.Role.IsValidSCP() && Round.Duration < TimeSpan.FromMinutes(1) && !Plugin.EventInProgress)
			{
				args.Player.SendBroadcast("You can swap SCP with another player by running the \".scpswap <SCP>\" command in your console", 5);
				args.Player.SendBroadcast("You can change back to a human role by running the \".human\" command", 5);
			}
		}

		[PluginEvent]
		public void RoundStart(RoundStartEvent args)
		{
			SwapManager.SCPsToReplace = 0;
			SwapManager.LateTimer = false;

		}

		[PluginEvent]
		public void RoundEnd(RoundEndEvent args)
		{
			SwapManager.SCPsToReplace = 0;
			SwapManager.LateTimer = false;
			SwapManager.triggers.Clear();
		}

		[PluginEvent]
		public void PlayerLeave(PlayerLeftEvent args)
		{
			if (Round.Duration < TimeSpan.FromMinutes(1) && args.Player.IsSCP)
			{
				SwapManager.SCPsToReplace++;
				SwapManager.ReplaceBroadcast();
			}
		}
	}
}
