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

namespace CustomCommands.Features.Humans.Disarming
{
	public class DisarmingEvents
	{
		[PluginEvent]
		public void PlayerDisarmed(PlayerHandcuffEvent args)
		{
			if (args.Target.Role != RoleTypeId.ClassD && args.Target.Role != RoleTypeId.Scientist)
				return;

			if (!args.Target.TemporaryData.Contains("cuffTokens"))
			{
				if (args.Target.Role == RoleTypeId.ClassD)
					Respawn.AddTickets(Respawning.SpawnableTeamType.NineTailedFox, 0.2f);
				else if (args.Target.Role == RoleTypeId.Scientist)
					Respawn.AddTickets(Respawning.SpawnableTeamType.ChaosInsurgency, 0.2f);

				args.Target.TemporaryData.Add("cuffTokens", string.Empty);
			}

			if (args.Target.Zone != FacilityZone.Surface)
			{
				if (args.Target.TemporaryData.Contains("cuffedBy"))
					args.Target.TemporaryData.Remove("cuffedBy");

				args.Target.TemporaryData.StoredData.Add("cuffedBy", args.Player.UserId);
			}
		}

		[PluginEvent]
		public void PlayerEscaped(PlayerEscapeEvent args)
		{
			if (args.Player.IsDisarmed && args.Player.TemporaryData.Contains("cuffedBy"))
			{
				Player cuffer = Player.Get(args.Player.TemporaryData.StoredData["cuffedBy"].ToString());

				if (cuffer.IsAlive)
				{
					var count = 1;
					if (cuffer.TemporaryData.Contains("rescueCount"))
					{
						count = (int)cuffer.TemporaryData.StoredData["rescueCount"];
						count++;

						cuffer.TemporaryData.StoredData["rescueCount"] = count;

					}
					else
						cuffer.TemporaryData.StoredData.Add("rescueCount", 1);

					if (count > 1)
					{
						var item = cuffer.Items.Where(i => i.ItemTypeId == ItemType.ArmorLight);
						if (item.Any())
						{
							cuffer.RemoveItem(item.First());
							cuffer.AddItem(ItemType.ArmorCombat);
						}

						item = cuffer.Items.Where(i => i.ItemTypeId == ItemType.KeycardGuard);
						if (item.Any())
						{
							cuffer.RemoveItem(item.First());
							cuffer.AddItem(ItemType.KeycardMTFOperative);
						}
					}

					foreach (ItemBase a in cuffer.Items)
					{
						if (a.Category == ItemCategory.Firearm && a is Firearm firearm)
						{
							switch (firearm.AmmoType)
							{
								case ItemType.Ammo12gauge:
									{
										ushort ammoCount = (ushort)UnityEngine.Random.Range(6, 15);
										cuffer.AddAmmo(ItemType.Ammo12gauge, ammoCount);
										break;
									}
								case ItemType.Ammo44cal:
									{
										ushort ammoCount = (ushort)UnityEngine.Random.Range(3, 9);
										cuffer.AddAmmo(ItemType.Ammo44cal, ammoCount);
										break;
									}
								case ItemType.Ammo556x45:
								case ItemType.Ammo762x39:
								case ItemType.Ammo9x19:
									{
										ushort ammoCount = (ushort)UnityEngine.Random.Range(20, 41);
										cuffer.AddAmmo(firearm.AmmoType, ammoCount);
										break;
									}
								default: break;
							}
						}


					}
					if (count > 1)
					{
						if (cuffer.Role == RoleTypeId.FacilityGuard && args.Player.Role == RoleTypeId.ClassD)
						{
							cuffer.ReferenceHub.roleManager.ServerSetRole(RoleTypeId.NtfSpecialist, RoleChangeReason.Escaped, RoleSpawnFlags.None);
						}
					}

					cuffer.ReceiveHint($"You have been rewarded for rescuing {args.Player.DisplayNickname}");
				}
			}
		}
	}
}
