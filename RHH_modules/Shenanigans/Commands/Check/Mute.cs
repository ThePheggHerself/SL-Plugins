using CommandSystem;
using RedRightHandCore.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceChat;

namespace Shenanigans.Commands.Check
{
	public class Mute : ICommand
	{
		public string Command => "mute";

		public string[] Aliases => null;

		public string Description => "Checks if a specific UserID is muted";

		public string[] Usage { get; } = { "UserID" };

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count < 1)
			{
				response = "Invalid UserID provided";
				return false;
			}

			var searchTerm = arguments.First();
			VoiceChatMutes.QueryLocalMute(searchTerm);

			if (VoiceChatMutes.QueryLocalMute(searchTerm))
				response = $"User {searchTerm} is muted";
			else
				response = $"User {searchTerm} is not muted";


			return true;
		}
	}
}
