using CommandSystem;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Reflection;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace CustomCommands.Features
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RagdollLauncher : ICustomCommand
	{
		public string Command => "ragdolllauncher";

		public string[] Aliases { get; } = { "rdl" };

		public string Description => "Launches ragdolls when you shoot your gun";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => PlayerPermissions.ServerConfigs;
		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out var pSender))
				return false;

			var plr = Player.Get(pSender.ReferenceHub);

			if (plr.TemporaryData.Contains("rdlauncher"))
				plr.TemporaryData.Remove("rdlauncher");
			else
				plr.TemporaryData.Add("rdlauncher", string.Empty);

			response = $"Ragdoll launcher {(plr.TemporaryData.Contains("rdlauncher") ? "enabled" : "disabled")}.";
			return true;
		}
	}

	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class BallLauncher : ICustomCommand
	{
		public string Command => "balllauncher";

		public string[] Aliases { get; } = { "bl" };

		public string Description => "Launches balls when you shoot your gun";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => PlayerPermissions.ServerConfigs;
		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out var pSender))
				return false;

			var plr = Player.Get(pSender.ReferenceHub);

			if (plr.TemporaryData.Contains("blauncher"))
				plr.TemporaryData.Remove("blauncher");
			else
				plr.TemporaryData.Add("blauncher", string.Empty);

			response = $"Ball launcher {(plr.TemporaryData.Contains("blauncher") ? "enabled" : "disabled")}.";
			return true;
		}
	}

	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class FlashLauncher : ICustomCommand
	{
		public string Command => "flashlauncher";

		public string[] Aliases { get; } = { "fl" };

		public string Description => "Launches flashbangs when you shoot your gun";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => PlayerPermissions.ServerConfigs;
		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out var pSender))
				return false;

			var plr = Player.Get(pSender.ReferenceHub);

			if (plr.TemporaryData.Contains("flauncher"))
				plr.TemporaryData.Remove("flauncher");
			else
				plr.TemporaryData.Add("flauncher", string.Empty);

			response = $"Flashbang launcher {(plr.TemporaryData.Contains("flauncher") ? "enabled" : "disabled")}.";
			return true;
		}
	}

	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GrenadeLauncher : ICustomCommand
	{
		public string Command => "grenadelauncher";

		public string[] Aliases { get; } = { "gl" };

		public string Description => "Launches grenades when you shoot your gun";

		public string[] Usage { get; } = { };

		public PlayerPermissions? Permission => PlayerPermissions.ServerConfigs;
		public string PermissionString => string.Empty;

		public bool RequirePlayerSender => true;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out var pSender))
				return false;

			var plr = Player.Get(pSender.ReferenceHub);

			if (plr.TemporaryData.Contains("glauncher"))
				plr.TemporaryData.Remove("glauncher");
			else
				plr.TemporaryData.Add("glauncher", string.Empty);

			response = $"Grenade launcher {(plr.TemporaryData.Contains("glauncher") ? "enabled" : "disabled")}.";
			return true;
		}
	}

	public class SpecialWeapons
	{
		RoleTypeId[] RagdollRoles = new RoleTypeId[]
		{
			RoleTypeId.ClassD, RoleTypeId.Scientist, RoleTypeId.Scp049, RoleTypeId.Scp0492, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor, RoleTypeId.ChaosRepressor,
			RoleTypeId.NtfCaptain, RoleTypeId.NtfSpecialist, RoleTypeId.NtfPrivate, RoleTypeId.NtfSergeant, RoleTypeId.Tutorial
		};

		public static Vector3 RandomThrowableVelocity(Transform transform)
		{
			Vector3 velocity = Vector3.zero;
			velocity += transform.forward * Random.Range(10f, 15f);
			velocity += transform.up * 1f;

			if (Random.Range(1, 3) % 2 == 0)
				velocity += transform.right * Random.Range(0.1f, 2.5f);

			else
				velocity += transform.right * -Random.Range(0.1f, 2.5f);

			return velocity;
		}

		[PluginEvent(ServerEventType.PlayerShotWeapon)]
		public void PlayerShootEvent(PlayerShotWeaponEvent args)
		{
			//NONE of this is ever going to end well. Like, at all.
			//My intrusive thoughts have won and this is now a thing...

			var plr = args.Player;

			if (plr.Role == RoleTypeId.Tutorial)
			{
				if (plr.TemporaryData.Contains("glauncher"))
				{
					ThrowableItem item = (ThrowableItem)plr.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext()), false);
					Vector3 Pos = plr.Position;
					Pos.y += 0.5f;

					TimeGrenade grenade = (TimeGrenade)Object.Instantiate(item.Projectile, Pos, Quaternion.identity);
					grenade.NetworkInfo = new InventorySystem.Items.Pickups.PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
					grenade.Position = Pos;
					grenade.Rotation = plr.GameObject.transform.rotation;
					grenade.GetComponent<Rigidbody>().velocity = RandomThrowableVelocity(args.Player.Camera.transform);

					grenade.PreviousOwner = new Footprinting.Footprint(plr.ReferenceHub);
					NetworkServer.Spawn(grenade.gameObject);
					grenade.ServerActivate();
				}

				else if (plr.TemporaryData.Contains("blauncher"))
				{
					ThrowableItem item = (ThrowableItem)plr.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.SCP018, ItemSerialGenerator.GenerateNext()), false);
					Vector3 Pos = plr.Position;
					Pos.y += 0.5f;

					Scp018Projectile grenade = (Scp018Projectile)UnityEngine.Object.Instantiate(item.Projectile, Pos, Quaternion.identity);
					grenade.NetworkInfo = new InventorySystem.Items.Pickups.PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
					grenade.Position = Pos;
					grenade.Rotation = plr.GameObject.transform.rotation;
					grenade.GetComponent<Rigidbody>().velocity = RandomThrowableVelocity(args.Player.Camera.transform);

					grenade.PreviousOwner = new Footprinting.Footprint(plr.ReferenceHub);
					NetworkServer.Spawn(grenade.gameObject);
					grenade.ServerActivate();
				}

				else if (plr.TemporaryData.Contains("flauncher"))
				{
					ThrowableItem item = (ThrowableItem)plr.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.GrenadeFlash, ItemSerialGenerator.GenerateNext()), false);
					Vector3 Pos = plr.Position;
					Pos.y += 0.5f;

					FlashbangGrenade grenade = (FlashbangGrenade)UnityEngine.Object.Instantiate(item.Projectile, Pos, Quaternion.identity);
					grenade.NetworkInfo = new InventorySystem.Items.Pickups.PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
					grenade.Position = Pos;
					grenade.Rotation = plr.GameObject.transform.rotation;
					grenade.GetComponent<Rigidbody>().velocity = RandomThrowableVelocity(args.Player.Camera.transform);

					grenade.PreviousOwner = new Footprinting.Footprint(plr.ReferenceHub);
					NetworkServer.Spawn(grenade.gameObject);
					grenade.ServerActivate();
				}

				else if (plr.TemporaryData.Contains("rdlauncher"))
				{
					var role = RagdollRoles[Random.Range(0, RagdollRoles.Length - 1)];

					PlayerRoleLoader.TryGetRoleTemplate(role, out FpcStandardRoleBase pRB);

					var dh = new FirearmDamageHandler(args.Firearm, 10);

					Vector3 velocity = Vector3.zero;
					velocity += args.Player.Camera.transform.forward * Random.Range(5f, 10f);
					velocity += args.Player.Camera.transform.up * Random.Range(0.75f, 4.5f);

					if (Random.Range(1, 3) % 2 == 0)
						velocity += args.Player.Camera.transform.right * Random.Range(0.1f, 2.5f);

					else
						velocity += args.Player.Camera.transform.right * -Random.Range(0.1f, 2.5f);

					typeof(StandardDamageHandler).GetField("StartVelocity", BindingFlags.NonPublic | BindingFlags.Instance)
						.SetValue(dh, velocity);

					RagdollData data = new RagdollData(null, dh, role, plr.Position, plr.GameObject.transform.rotation, plr.Nickname, NetworkTime.time);
					BasicRagdoll basicRagdoll = Object.Instantiate(pRB.Ragdoll);
					basicRagdoll.NetworkInfo = data;
					NetworkServer.Spawn(basicRagdoll.gameObject, (NetworkConnection)null);
				}
			}
		}
	}
}
