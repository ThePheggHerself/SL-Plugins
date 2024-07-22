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
	}
}
