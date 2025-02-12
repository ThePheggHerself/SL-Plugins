using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenanigans.Commands.Spawn
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SpawnParent : ParentCommand
	{
		public override string Command => throw new NotImplementedException();

		public override string[] Aliases => throw new NotImplementedException();

		public override string Description => throw new NotImplementedException();

		public override void LoadGeneratedCommands()
		{
			RegisterCommand(new Grenade());
			RegisterCommand(new Flashbang());
			RegisterCommand(new Ball());
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please provide a valid subcommand. grenade/flash/ball";
			return false;
		}
	}
}
