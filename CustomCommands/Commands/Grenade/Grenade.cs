using CommandSystem;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PluginAPI.Core;
using System;
using UnityEngine;

namespace CustomCommands.Commands
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GrenadeCommand : ICustomCommand
	{
		public string Command => "grenade";

		public string[] Aliases => null;

		public string Description => "Spawns a grenade at a player's location";

		public string[] Usage { get; } = { "%player%" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.grenade";

		public bool RequirePlayerSender => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out var players, out _))
				return false;

			foreach (Player plr in players)
			{
				if (plr.Role == PlayerRoles.RoleTypeId.Spectator || plr.Role == PlayerRoles.RoleTypeId.Overwatch)
					continue;

				ThrowableItem item = (ThrowableItem)plr.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext()), false);
				Vector3 Pos = plr.Position;
				Pos.y += 1;

				TimeGrenade grenade = (TimeGrenade)UnityEngine.Object.Instantiate(item.Projectile, Pos, Quaternion.identity);
				grenade.NetworkInfo = new InventorySystem.Items.Pickups.PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
				grenade.Position = Pos;
				grenade.Rotation = Quaternion.identity;
				grenade.GetComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1));

				grenade.PreviousOwner = new Footprinting.Footprint(plr.ReferenceHub);
				NetworkServer.Spawn(grenade.gameObject);
				grenade.ServerActivate();
			}

			response = $"Spawned a grenade on {players.Count} {(players.Count > 1 ? "players" : "player")}";
			return true;
		}
	}
}
