using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendlyFireDetector.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class FFDParent : ParentCommand
	{
		public override string Command => "rrhffd";

		public override string[] Aliases => Array.Empty<string>();

		public override string Description => "Commands to handle the Friendly Fire Detector module of RedRightHand";

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new Pause());
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please provide a valid subcommand. Pause";
			return false;
		}
	}
}
