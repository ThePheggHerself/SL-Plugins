using CommandSystem;
using CustomCommands.Events;
using Hints;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using MapGeneration;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.RoleAssign;
using PlayerStatsSystem;
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

namespace CustomCommands.Features.Humans.Disarming
{
	public class DisarmingEvents
	{
		[PluginEvent]
		public void PlayerDisarmed(PlayerHandcuffEvent args)
		{
			if(args.Target.Role == RoleTypeId.ClassD && !args.Target.TemporaryData.Contains("kosdisarm"))
			{
				args.Target.TemporaryData.StoredData.Add("kosdisarm", (int)1);
			}
		}

		[PluginEvent]
		public void PlayerDamaged(PlayerDamageEvent args)
		{
			if (args.DamageHandler is FirearmDamageHandler fDH)
			{
				var isVicClassD = args.Target.Role == RoleTypeId.ClassD;
				var isAtkrFacGuard = args.Player.Role == RoleTypeId.FacilityGuard;
				var hasVicDisarmed = !args.Target.TemporaryData.Contains("kosdisarm");
				var hasExclusionItems = !args.Target.ReferenceHub.inventory.UserInventory.Items.Where(i => 
					i.Value.Category == ItemCategory.Firearm || 
					i.Value.Category == ItemCategory.SpecialWeapon || 
					(i.Value.Category == ItemCategory.SCPItem && i.Value.ItemTypeId != ItemType.SCP330) || 
					i.Value.Category == ItemCategory.Grenade).Any();

				if (isVicClassD && isAtkrFacGuard && hasVicDisarmed && hasExclusionItems)
				{
					var propinfo = fDH.GetType().GetProperty("Damage", Plugin.BindingFlags);
					propinfo.SetValue(args.DamageHandler, fDH.Damage / 2);
				}
			}
		}
	}
}
