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

namespace CustomCommands.Features.Items
{
	public static class ItemManager
	{
		public static ushort TranqGunSerial = 0;
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

		public static void SpawnGrenade<T>(Player thrower, ItemType Item) where T : TimeGrenade =>
			SpawnGrenade<T>(thrower, Item, new Vector3(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1)));

		public static void SpawnGrenade<T>(Player thrower, ItemType Item, Vector3 Direction) where T : TimeGrenade
		{
			ThrowableItem item = (ThrowableItem)thrower.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(Item, ItemSerialGenerator.GenerateNext()), false);
			Vector3 Pos = thrower.Position;
			Pos.y += 1;

			T grenade = (T)UnityEngine.Object.Instantiate(item.Projectile, Pos, Quaternion.identity);
			grenade.NetworkInfo = new InventorySystem.Items.Pickups.PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
			grenade.Position = Pos;
			grenade.Rotation = Quaternion.identity;
			grenade.GetComponent<Rigidbody>().velocity = Direction;

			grenade.PreviousOwner = new Footprinting.Footprint(thrower.ReferenceHub);
			Mirror.NetworkServer.Spawn(grenade.gameObject);
			grenade.ServerActivate();
		}
	}
}
