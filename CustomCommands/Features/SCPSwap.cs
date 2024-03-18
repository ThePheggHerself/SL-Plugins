using CommandSystem;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using RemoteAdmin;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class CmdSCPList : ICommand
	{
		public string Command => "scplist";

		public string[] Aliases { get; } = { "slist" };
		public string Description => "Lists all current SCPs";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && pSender.ReferenceHub.IsSCP())
			{
				var plr = Player.Get(pSender.ReferenceHub);

				var scps = Player.GetPlayers().Where(r => r.IsSCP);

				if (!scps.Any())
				{
					response = "There are no other SCPs";
					return false;
				}

				List<string> scpString = new List<string>();
				foreach (var a in scps)
				{
					scpString.Add(a.Role.ToString().ToLower().Replace("scp", string.Empty));
				}

				response = $"Current SCPs: {string.Join(", ", scpString)}";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}

	[CommandHandler(typeof(ClientCommandHandler))]
	public class CmdSCPSwap : ICustomCommand
	{
		public string Command => "scpswap";

		public string[] Aliases { get; } = { "sswap" };
		public string Description => "Changes your current SCP";

		public string[] Usage { get; } = { "scp" };

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && pSender.ReferenceHub.IsSCP() && pSender.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp0492)
			{
				var player = Player.Get(pSender.ReferenceHub);

				if (player.Health != player.MaxHealth)
				{
					response = "You cannot swap as you have taken damage";
					return false;
				}
				else if (Round.Duration > TimeSpan.FromMinutes(1))
				{
					response = "You can only swap your SCP within the first minute of a round";
					return false;
				}

				var role = Extensions.GetRoleFromString($"SCP" + arguments.Array[1]);
				if (SCPSwap.AvailableSCPs.Contains(role))
				{
					response = "You cannot swap to that SCP";
					return false;
				}

				var scpNum = player.Role.SCPNumbersFromRole();
				var target = Player.GetPlayers().Where(r => r.Role == role).First();

				if (player.TemporaryData.Contains("swapRequestSent"))
				{
					response = "You already have another pending swap request";
					return false;
				}
				else if (player.TemporaryData.Contains("swapRequestRecieved"))
				{
					response = "You must reject your pending request before trying to swap with another SCP";
					return false;
				}
				else if (target.TemporaryData.Contains("swapRequestSent"))
				{
					response = $"{target} is trying to swap with another player";
					return false;
				}
				else if (target.TemporaryData.Contains("swapRequestRecieved"))
				{
					response = $"{target} already has a pending swap request";
					return false;
				}

				target.ReceiveHint($"{player.Nickname} wants to swap SCP with you. Type `.sswapa` in your console to swap to SCP-{scpNum}, or type `.sswapd` to reject the request", 8);
				target.SendConsoleMessage($"{player.Nickname} wants to swap SCP with you. Type `.sswapa` in your console to swap to SCP-{scpNum}, or type `.sswapd` to reject the request");
				target.TemporaryData.Add("swapRequestRecieved", player.UserId);
				player.TemporaryData.Add("swapRequestSent", target.UserId);

				response = "Swap Request Sent";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}

	[CommandHandler(typeof(ClientCommandHandler))]
	public class CmcSCPSwapAccept : ICustomCommand
	{
		public string Command => "scpswapaccept";

		public string[] Aliases { get; } = { "sswapa", "ssa" };
		public string Description => "Accepts your pending swap request";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && pSender.ReferenceHub.IsSCP() && pSender.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp0492)
			{
				var player = Player.Get(pSender.ReferenceHub);

				if (!player.TemporaryData.TryGet("swapRequestRecieved", out string UserId))
				{
					response = "You do not have a pending swap request";
					return false;
				}

				if (!Player.TryGet(UserId, out Player swapper))
				{
					response = "Unable to find request sender. Cancelling request";
					player.TemporaryData.Remove("swapRequestRecieved");
					return false;
				}

				RoleTypeId playerSCP = player.Role;
				RoleTypeId swapperSCP = swapper.Role;

				swapper.ReceiveHint($"{player.Nickname} accepted your swap request", 5);

				player.SetRole(swapperSCP, RoleChangeReason.LateJoin);
				swapper.SetRole(playerSCP, RoleChangeReason.LateJoin);

				player.TemporaryData.Remove("swapRequestRecieved");
				swapper.TemporaryData.Remove("swapRequestSent");

				response = "Request accepted";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}

	[CommandHandler(typeof(ClientCommandHandler))]
	public class CmdSCPSwapDeny : ICustomCommand
	{
		public string Command => "scpswapdeny";

		public string[] Aliases { get; } = { "sswapd", "ssd" };
		public string Description => "Denies your pending swap request";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && pSender.ReferenceHub.IsSCP() && pSender.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp0492)
			{
				var player = Player.Get(pSender.ReferenceHub);

				if (!player.TemporaryData.TryGet("swapRequestRecieved", out string UserId))
				{
					response = "You do not have a pending swap request";
					return false;
				}

				if (!Player.TryGet(UserId, out Player swapper))
				{
					response = "Unable to find request sender. Cancelling request";
					player.TemporaryData.Remove("swapRequestRecieved");
					return false;
				}

				swapper.ReceiveHint($"{player.Nickname} denied your swap request", 5);

				player.TemporaryData.Remove("swapRequestRecieved");
				swapper.TemporaryData.Remove("swapRequestSent");

				response = "Request denied";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}

	[CommandHandler(typeof(ClientCommandHandler))]
	public class CmdSwapToHuman : ICustomCommand
	{
		public string Command => "human";

		public string[] Aliases => null;
		public string Description => "Changes you back to a human role";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender && pSender.ReferenceHub.IsSCP() && pSender.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp0492)
			{
				var player = Player.Get(pSender.ReferenceHub);

				if (player.Health != player.MaxHealth)
				{
					response = "You cannot swap as you have taken damage";
					return false;
				}
				if (Round.Duration > TimeSpan.FromMinutes(1))
				{
					response = "You can only swap from SCP within the first 1 minute of a round";
					return false;
				}

				SCPSwap.SCPsToReplace++;
				HumanSpawner.SpawnLate(pSender.ReferenceHub);
				player.TemporaryData.Add("startedasscp", true.ToString());
				SCPSwap.ReplaceBroadcast();

				response = "You have now swapped to Human from SCP";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}

	[CommandHandler(typeof(ClientCommandHandler))]
	public class CmdSwapFromHuman : ICustomCommand
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

				if (SCPSwap.SCPsToReplace < 1)
				{
					response = "There are no SCPs to replace";
					return false;
				}
				if (player.TemporaryData.Contains("startedasscp"))
				{
					response = "You were already an SCP this round";
					return false;
				}
				if (Round.Duration > TimeSpan.FromSeconds(90) && !SCPSwap.LateTimer || Round.Duration > TimeSpan.FromSeconds(120))
				{
					response = "You can only swap within the first 90 seconds of the round";
					return false;
				} 

				if (SCPSwap.Cooldown.TryGetValue(player.UserId, out int roundCount) && (RoundRestart.UptimeRounds - roundCount) < 3)
				{
					response = "You have already recently replaced an SCP and are still on cooldown";
					return false;
				}

				var scps = SCPSwap.AvailableSCPs;

				player.SetRole(scps[new Random().Next(0, scps.Length)], RoleChangeReason.LateJoin);

				SCPSwap.SCPsToReplace--;

				if (SCPSwap.Cooldown.ContainsKey(player.UserId))
					SCPSwap.Cooldown[player.UserId] = RoundRestart.UptimeRounds;
				else
					SCPSwap.Cooldown.Add(player.UserId, RoundRestart.UptimeRounds);

				response = "You have replaced an SCP";
				return true;
			}

			response = "You must be a living human role to run this command";
			return false;
		}
	}

	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CmdTriggerReplace : ICustomCommand
	{
		public string Command => "replacescp";

		public string[] Aliases => null;
		public string Description => "Manually triggers the SCP replacement broadcast";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			SCPSwap.SCPsToReplace++;
			SCPSwap.ReplaceBroadcast();
			SCPSwap.LateTimer = true;
			response = "SCP replace triggered";
			return true;
		}
	}



	public class SCPSwap
	{
		public static int SCPsToReplace = 0;
		public static void ReplaceBroadcast() => Server.SendBroadcast($"There {(SCPsToReplace == 1 ? "is" : "are")} now {SCPsToReplace} SCP spot{(SCPsToReplace == 1 ? "" : "s")} available. Run \".scp\" to queue for an SCP", 5);
		public static bool LateTimer = false;

		public static RoleTypeId[] AvailableSCPs
		{
			get
			{
				var Roles = new List<RoleTypeId>() { RoleTypeId.Scp049, /*RoleTypeId.Scp079, */RoleTypeId.Scp106, RoleTypeId.Scp173, RoleTypeId.Scp939, RoleTypeId.Scp096 };

				var scpRoles = Player.GetPlayers().Where(r => r.ReferenceHub.IsSCP()).Select(r => r.Role);
				//if (scpRoles.Any())
				foreach (var r in scpRoles)
				{
					if (Roles.Contains(r))
						Roles.Remove(r);
				}
				//else
				//    Roles.Remove(RoleTypeId.Scp079);

				return Roles.ToArray();
			}
		}

		public static Dictionary<string, int> Cooldown = new Dictionary<string, int>();

		[PluginEvent(ServerEventType.PlayerSpawn)]
		public void PlayerSpawn(PlayerSpawnEvent args)
		{
			if (args.Player.Role.IsValidSCP() && Round.Duration < TimeSpan.FromMinutes(1) && !Plugin.EventInProgress)
			{
				args.Player.SendBroadcast("You can swap SCP with another player by running the \".scpswap <SCP>\" command in your console", 5);
				args.Player.SendBroadcast("You can change back to a human role by running the \".human\" command", 5);
			}
		}

		[PluginEvent(ServerEventType.RoundStart)]
		public void RoundStart(RoundStartEvent args)
		{
			SCPsToReplace = 0;
            LateTimer = false;

        }

        [PluginEvent(ServerEventType.RoundEnd)]
		public void RoundEnd(RoundEndEvent args)
		{
			SCPsToReplace = 0;
			LateTimer = false;
		}

		[PluginEvent(ServerEventType.PlayerLeft)]
		public void PlayerLeave(PlayerLeftEvent args)
		{
			if (Round.Duration < TimeSpan.FromMinutes(1) && args.Player.IsSCP)
			{
				SCPsToReplace++;
				ReplaceBroadcast();
			}
		}
	}
}
