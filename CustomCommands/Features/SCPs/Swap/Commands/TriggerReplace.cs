using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.SCPs.Swap.Commands
{

	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class TriggerReplace : ICustomCommand
	{
		public string Command => "replacescp";

		public string[] Aliases => null;
		public string Description => "Manually triggers the SCP replacement broadcast";

		public string[] Usage => null;

		public PlayerPermissions? Permission => null;

		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			SwapManager.SCPsToReplace++;
			SwapManager.ReplaceBroadcast();
			SwapManager.LateTimer = true;
			response = "SCP replace triggered";
			return true;
		}
	}
}
