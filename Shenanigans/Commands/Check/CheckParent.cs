using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Commands.Check
{
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CheckParent : ParentCommand
	{
		public override string Command => "check";

		public override string[] Aliases => [];

		public override string Description => "Check info for an offline player";

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new Ban());
			RegisterCommand(new Mute());
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please provide a valid subcommand";
			return false;
		}
	}
}
