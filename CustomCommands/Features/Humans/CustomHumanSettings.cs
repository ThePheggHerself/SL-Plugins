using CustomCommands.Features.SCPs.Swap;
using CustomCommands.ServerSettings;
using InventorySystem;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UserSettings.ServerSpecific;
using Utils;

namespace CustomCommands.Features.Humans
{
	public class CustomHumanSettings : CustomSettingsBase
	{
		public override string Name => "Human Settings";

		public override string Description => "Various settings for custom human controls";

		public override ServerSpecificSettingBase[] SettingBases => new ServerSpecificSettingBase[]
		{
			new SSGroupHeader(Name, false, Description),
			new SSKeybindSetting((int)CustomSettingsManager.SettingsIDs.Human_HealOther, "Heal Player", KeyCode.K, true, "Heals the player you are currently looking at (Requires a medkit)"),
			new SSKeybindSetting((int)CustomSettingsManager.SettingsIDs.Human_Suicide, "KABOOM!", KeyCode.KeypadEquals, true, "KABOOM! :)")
		};

		public override void Activate()
		{
			ServerSpecificSettingsSync.ServerOnSettingValueReceived += this.ProcessUserInput;
		}

		public override void Deactivate()
		{
			ServerSpecificSettingsSync.ServerOnSettingValueReceived -= this.ProcessUserInput;
		}

		public override void ProcessUserInput(ReferenceHub hub, ServerSpecificSettingBase setting)
		{
			switch (setting.SettingId)
			{
				case (int)CustomSettingsManager.SettingsIDs.Human_Suicide:
					{
						if (setting is SSKeybindSetting kbSetting && kbSetting.SyncIsPressed && hub.IsHuman())
						{
							ExplosionUtils.ServerSpawnEffect(hub.GetPosition(), ItemType.GrenadeHE);
							hub.playerStats.DealDamage(new ExplosionDamageHandler(new Footprinting.Footprint(hub), hub.PlayerCameraReference.up * 2, 1000, 1000, ExplosionType.Grenade));
						}
						break;
					}

				case (int)CustomSettingsManager.SettingsIDs.Human_HealOther:
					{
						if (setting is SSKeybindSetting kbSetting && kbSetting.SyncIsPressed)
						{
							if (hub.inventory.CurItem.TypeId == ItemType.Medkit)
							{
								if(Physics.Raycast(hub.PlayerCameraReference.position, hub.PlayerCameraReference.forward, out RaycastHit info, 3.5f)){
									if(info.collider.TryGetComponent(out HitboxIdentity hbI) && !HitboxIdentity.IsEnemy(hbI.TargetHub, hub) && hbI.TargetHub != hub)
									{
										hbI.TargetHub.playerStats.GetModule<HealthStat>().ServerHeal(40);
										hub.inventory.ServerRemoveItem(hub.inventory.CurItem.SerialNumber, null);
									}
								}
							}
						}

						break;
					}

				default:
					break;
			}
		}
	}
}
