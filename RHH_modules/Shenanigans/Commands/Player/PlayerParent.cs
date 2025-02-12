using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shenanigans.Commands.Player
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PlayerParent : ParentCommand
	{
		public override string Command => "player";

		public override string[] Aliases { get; } = { "plr" };

		public override string Description => "Various commands targetted at players";

		public override void LoadGeneratedCommands()
		{
			throw new NotImplementedException();
		}

		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = string.Join("/", this.Commands.Select(r => r.Value.Command));
			return false;
		}
	}
}
