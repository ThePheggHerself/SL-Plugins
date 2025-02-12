using CommandSystem;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using PlayerRoles;
using RedRightHandCore;
using RedRightHandCore.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.NonAllocLINQ;

namespace CustomCommands.Commands.Renaming
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class MassUnname : ICustomCommand
	{
		public string Command => "massunname";

		public string[] Aliases { get; } = { "unnameall" };

		public string Description => "Resets every players' nickname";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => PlayerPermissions.PlayersManagement;
		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			players = Player.List.ToList();

			int c = 0;
			foreach (Player plr in players)
			{
				plr.DisplayName = "";
				c++;
			}

			response = $"Reset the nicknames of {c} players";

			return true;
		}


	}
}
