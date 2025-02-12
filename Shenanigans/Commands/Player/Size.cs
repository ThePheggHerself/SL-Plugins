using CommandSystem;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Features.Wrappers;
using RedRightHandCore;
using RedRightHandCore.Commands;
using System;
using UnityEngine;

namespace CustomCommands.Commands.Plr
{
	public class SizeCommand : ICustomCommand
	{
		public string Command => "size";

		public string[] Aliases { get; } = { "scale" };
		public string Description => "Modify the size of a specified player";
		public string[] Usage { get; } = { "%player%", "x", "y", "z" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.size";
		public bool RequirePlayerSender => true;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			if (!float.TryParse(arguments.Array[2], out float x) || !float.TryParse(arguments.Array[3], out float y) || !float.TryParse(arguments.Array[4], out float z))
			{
				response = "Valid scale not provided";
				return false;
			}

			foreach (var p in players)
			{
				p.ReferenceHub.transform.localScale = new Vector3(x, y, z);
			}

			response = $"Scale of {players.Count} {(players.Count != 1 ? "players" : "player")} has been set to {x}, {y}, {z}";
			return true;
		}
	}
}
