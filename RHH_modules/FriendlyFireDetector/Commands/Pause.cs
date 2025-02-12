using CommandSystem;
using LabApi.Loader.Features.Plugins;
using RedRightHandCore.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendlyFireDetector.Commands
{
	[CommandHandler(typeof(FFDParent))]
	public class Pause : ICommand
	{
		public string Command => "pause";

		public string[] Aliases => Array.Empty<string>();

		public string Description => "Pause the FFD";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			Events.FFDPaused = !Events.FFDPaused;

			response = $"Friendly Fire Detector is {(Events.FFDPaused ? "now" : "no longer")} paused";
			return true;
		}
	}
}
