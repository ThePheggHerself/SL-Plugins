using CommandSystem;
using CustomCommands.Events;
using Hints;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms;
using MapGeneration;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.RoleAssign;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Items;
using PluginAPI.Enums;
using PluginAPI.Events;
using RemoteAdmin;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MEC;
using HarmonyLib;
using PluginAPI.Core.Zones;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles.PlayableScps;
using System.Reflection;
using InventorySystem.Items.Pickups;

namespace CustomCommands.Features
{
	public class _079Removal
	{
		//[PluginEvent]
		public void SpawnEvent(PlayerSpawnEvent args)
		{
			var scps = SCPSwap.AvailableSCPs;

			if (args.Role == RoleTypeId.Scp079 && scps.Any())
			{
				Timing.CallDelayed(0.15f, () =>
				{
					args.Player.SetRole(scps[new System.Random().Next(0, scps.Length)], RoleChangeReason.LateJoin);
				});
			}
		}

		[PluginEvent]
		public bool PlayerInteractDoor(PlayerInteractDoorEvent args)
		{
			if (!Plugin.EventInProgress && RoomLightController.IsInDarkenedRoom(args.Player.Position))
				return false;

			return args.CanOpen;
		}

		[PluginEvent]
		public void RoundStartEvent(RoundStartEvent args)
		{
			Timing.RunCoroutine(LightFailure());
		}

		public IEnumerator<float> LightFailure()
		{
			var delay = UnityEngine.Random.Range(90, 151);


			yield return Timing.WaitForSeconds(delay);

			Cassie.Message("Attention all personnel . Power malfunction detected . Repair protocol delta 12 activated . Heavy containment zone power termination in 3 . 2 . 1", false, true, true);
			yield return Timing.WaitForSeconds(18f);

			foreach (RoomLightController instance in RoomLightController.Instances)
			{
				if (instance.Room.Zone == MapGeneration.FacilityZone.HeavyContainment)
				{
					instance.ServerFlickerLights(30);
				}
			}

			foreach (var door in DoorVariant.AllDoors.Where(r => r.IsInZone(MapGeneration.FacilityZone.HeavyContainment)))
			{
				var a = door is IDamageableDoor iDD && door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None && !door.name.Contains("LCZ");

				//Log.Info($"{door.name} {door.RequiredPermissions.RequiredPermissions} || {door.Rooms.Length} {door.Rooms.First().name} {a}");
				//Log.Info(door.name + " " + door.RequiredPermissions.RequiredPermissions + " |||| " + door.Rooms.Length + " " + door.Rooms.First().name + " ");
				if (a)
				{
					door.NetworkTargetState = true;
				}

			}

			foreach (var tesla in TeslaGateController.Singleton.TeslaGates)
			{
				tesla.enabled = false;
			}

			yield return Timing.WaitForSeconds(30f);

			Cassie.Message("Power system repair complete . System back online", false, true, true);

			foreach (var tesla in TeslaGateController.Singleton.TeslaGates)
			{
				tesla.enabled = true;
			}

			yield return 0f;
		}


	}
}
