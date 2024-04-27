using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using CustomCommands.Features.Ragdoll;
using PluginAPI.Core.Items;
using InventorySystem;
using PluginAPI.Core;
using InventorySystem.Items.Firearms;

namespace CustomCommands.Features.Items.Weapons
{
	public class WeaponEvents
	{
		RoleTypeId[] RagdollRoles = new RoleTypeId[]
		{
			RoleTypeId.ClassD, RoleTypeId.Scientist, RoleTypeId.Scp049, RoleTypeId.Scp0492, RoleTypeId.ChaosConscript, RoleTypeId.ChaosMarauder, RoleTypeId.ChaosRepressor, RoleTypeId.ChaosRepressor,
			RoleTypeId.NtfCaptain, RoleTypeId.NtfSpecialist, RoleTypeId.NtfPrivate, RoleTypeId.NtfSergeant, RoleTypeId.Tutorial
		};

		[PluginEvent]
		public void PlayerShootEvent(PlayerShotWeaponEvent args)
		{
			var plr = args.Player;

			if (plr.Role == RoleTypeId.Tutorial)
			{
				if (plr.TemporaryData.Contains("flauncher") && Plugin.Config.EnableFlashbangLauncher)
				{
					ItemManager.SpawnGrenade<FlashbangGrenade>(plr, ItemType.GrenadeFlash, ItemManager.RandomThrowableVelocity(args.Player.Camera.transform));
				}
				else if (plr.TemporaryData.Contains("glauncher") && Plugin.Config.EnableGrenadeLauncher)
				{
					ItemManager.SpawnGrenade<TimeGrenade>(plr, ItemType.GrenadeHE, ItemManager.RandomThrowableVelocity(args.Player.Camera.transform));
				}
				else if (plr.TemporaryData.Contains("blauncher") && Plugin.Config.EnableBallLauncher)
				{
					ItemManager.SpawnGrenade<Scp018Projectile>(plr, ItemType.SCP018, ItemManager.RandomThrowableVelocity(args.Player.Camera.transform));
				}

				else if (plr.TemporaryData.Contains("rdlauncher") && Plugin.Config.EnableRagdollLauncher)
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

		[PluginEvent]
		public bool PlayerAttackEvent(PlayerDamageEvent ev)
		{
			if (ev.Player == null)
				return true;

			if (ev.Player.CurrentItem.ItemTypeId == ItemType.GunCOM18)
			{
				ev.Target.RagdollPlayerTranqGun(ev.Player, 4, 2);
				return false;
			}

			return true;
		}

		[PluginEvent]
		public bool PlayerReloadEvent(PlayerReloadWeaponEvent ev)
		{
			if(ev.Firearm.ItemTypeId == ItemType.GunCOM18)
			{
				ev.Player.ReceiveHint("You cannot reload this weapon");
				return false;
			}
			return true;
		}

		[PluginEvent]
		public void PlayerChangeItemEvent(PlayerChangeItemEvent ev)
		{
			if (!Round.IsRoundStarted)
				return;

			if(ev.Player.CurrentItem != null && ev.Player.CurrentItem.ItemTypeId == ItemType.GunCOM18)
			{
				ev.Player.ReceiveHint("You equipped the tranquilizer. Humans who are shot will be ragdolled for a few seconds. This gun cannot be reloaded", 8);
			}
		}
	}
}
