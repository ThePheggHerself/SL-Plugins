#pragma warning disable IDE0059 // Unnecessary assignment of a value
using CommandSystem;
using PluginAPI.Core;
using System;
using System.Reflection;

namespace CustomCommands.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class TestCommand : ICustomCommand
	{
		public string Command => "nevergonna";

		public string[] Aliases => null;

		public string Description => "Test command. Try it :)";

		public string[] Usage => new[] { "give"/*, "you", "up"*/ };

		public PlayerPermissions? Permission => null;

		public string PermissionString => "test";

		public bool RequirePlayerSender => true;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out var pSender))
				return false;

			Log.Info("AA");

			int index = 0;

			Log.Info($"{pSender?.ReferenceHub == null} {pSender?.ReferenceHub?.gameObject == null}");

			foreach (var a in pSender.ReferenceHub.roleManager.CurrentRole.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				Log.Info($"{index++} {a.Name} | {a.GetType()} |");
			}



			response = $"Never gonna give you up,\nNever gonna let you down.\nNever gonna run around,\nAnd desert you.\nNever gonna make you cry,\nNever gonna say goodbye.\nNever gonna tell a lie,\nAnd hurt you.";
			return true;
		}
	}
}
