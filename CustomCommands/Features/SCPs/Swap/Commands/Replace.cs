using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using RemoteAdmin;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.SCPs.Swap.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Replace : ICustomCommand
	{
		public string Command => "scp";

		public string[] Aliases => null;
		public string Description => "Replaces you with an SCP who disconnected or swapped to human";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && !pSender.ReferenceHub.IsSCP() && pSender.ReferenceHub.IsAlive())
			{
				var player = Player.Get(pSender.ReferenceHub);

				if (SwapManager.SCPsToReplace < 1)
				{
					response = "There are no SCPs to replace";
					return false;
				}
				if (player.TemporaryData.Contains("startedasscp"))
				{
					response = "You were already an SCP this round";
					return false;
				}
				if (Round.Duration > TimeSpan.FromSeconds(90) && !SwapManager.LateTimer || Round.Duration > TimeSpan.FromSeconds(120))
				{
					response = "You can only swap within the first 90 seconds of the round";
					return false;
				}

				if (SwapManager.Cooldown.TryGetValue(player.UserId, out int roundCount) && (RoundRestart.UptimeRounds - roundCount) < 3)
				{
					response = "You have already recently replaced an SCP and are still on cooldown";
					return false;
				}

				var scps = SwapManager.AvailableSCPs;

				player.SetRole(scps[new Random().Next(0, scps.Length)], RoleChangeReason.LateJoin);

				SwapManager.SCPsToReplace--;

				if (SwapManager.Cooldown.ContainsKey(player.UserId))
					SwapManager.Cooldown[player.UserId] = RoundRestart.UptimeRounds;
				else
					SwapManager.Cooldown.Add(player.UserId, RoundRestart.UptimeRounds);

				response = "You have replaced an SCP";
				return true;
			}

			response = "You must be a living human role to run this command";
			return false;
		}
	}
}
