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

namespace CustomCommands.Features.SCPs.SCP079Removal
{
	public class RemovalEvents
	{
		public static bool Blackoutable = false;
		[PluginEvent]
		public void SpawnEvent(PlayerSpawnEvent args)
		{
			var scps = Swap.SwapManager.AvailableSCPs;

			if (args.Role == RoleTypeId.Scp079)
			{
				Timing.CallDelayed(0.15f, () =>
				{
					args.Player.SetRole(RoleTypeId.Scp3114, RoleChangeReason.LateJoin);
				});
			}
		}

		[PluginEvent]
		public bool PlayerInteractDoor(PlayerInteractDoorEvent args)
		{
			if (!Plugin.EventInProgress && RoomLightController.IsInDarkenedRoom(args.Player.Position) && args.Door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
				return false;

			return args.CanOpen;
		}

		[PluginEvent]
		public void RoundStartEvent(RoundStartEvent args)
		{
			Blackoutable = true;
			Timing.RunCoroutine(LightFailure());
		}

		public IEnumerator<float> LightFailure()
		{
			var delay = UnityEngine.Random.Range(180, 360);

			yield return Timing.WaitForSeconds(delay);

			if (Round.IsRoundEnded)
				yield return 0f;
			
			if (Blackoutable && Round.Duration >= TimeSpan.FromSeconds(120))
			{

			Blackoutable = false;

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
}
