using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Dummy.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Destroy : ICustomCommand
	{
		public string Command => "dummydestroy";

		public string[] Aliases => null;

		public string Description => "Destroys a dummy player";

		public string[] Usage { get; } = { "User ID" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.dummyd";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			if (DummyManager.DestroyDummy(arguments.ElementAt(0)))
			{
				response = $"Dummy '{arguments.ElementAt(0)}' destroyed";
				return true;
			}
			else
			{
				response = $"No dummy located";
				return false;
			}
		}
	}
}
