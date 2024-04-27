using CommandSystem;
using PlayerRoles.RoleAssign;
using PlayerRoles;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.SCPs.Swap.Commands
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class Human : ICustomCommand
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

				SwapManager.SCPsToReplace++;
				HumanSpawner.SpawnLate(pSender.ReferenceHub);
				player.TemporaryData.Add("startedasscp", true.ToString());
				SwapManager.ReplaceBroadcast();

				response = "You have now swapped to Human from SCP";
				return true;
			}

			response = "You must be an SCP to run this command";
			return false;
		}
	}
}
