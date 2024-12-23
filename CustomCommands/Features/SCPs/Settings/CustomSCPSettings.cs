using CustomCommands.Features.SCPs.Swap;
using CustomCommands.ServerSettings;
using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UserSettings.ServerSpecific;
using UserSettings.ServerSpecific.Examples;
using UserSettings.UserInterfaceSettings;

namespace CustomCommands.Features.SCPs.Settings
{
	public class CustomSCPSettings : CustomSettingsBase
	{
		public override string Name => "SCP Settings";
		public override string Description => "Various settings for our SCP swap system";
		public override ServerSpecificSettingBase[] SettingBases => new ServerSpecificSettingBase[]
		{
			new SSGroupHeader(Name, false, Description),
			new SSKeybindSetting((int) CustomSettingsManager.SettingsIDs.SCP_SwapToHuman, "Swap to Human", KeyCode.O, true, "Swap from an SCP to a Human role."),
			new SSKeybindSetting((int) CustomSettingsManager.SettingsIDs.SCP_SwapFromHuman, "Swap to SCP", KeyCode.L, true, "Replaces an SCP if an SCP slot is available."),
			new SSTwoButtonsSetting((int) CustomSettingsManager.SettingsIDs.SCP_NeverSCP, "Never Spawn as SCP", "Enabled", "Disabled", true, "Always get swapped to a human role if you spawn as an SCP")
		};

		public override void Activate()
		{
			ServerSpecificSettingsSync.ServerOnSettingValueReceived += this.ProcessUserInput;
			ReferenceHub.OnPlayerRemoved = (Action<ReferenceHub>)Delegate.Combine(ReferenceHub.OnPlayerRemoved, new Action<ReferenceHub>(this.OnPlayerDisconnected));
		}

		public override void Deactivate()
		{
			ServerSpecificSettingsSync.ServerOnSettingValueReceived -= this.ProcessUserInput;
			ReferenceHub.OnPlayerRemoved = (Action<ReferenceHub>)Delegate.Remove(ReferenceHub.OnPlayerRemoved, new Action<ReferenceHub>(this.OnPlayerDisconnected));
		}

		private void OnPlayerDisconnected(ReferenceHub hub)
		{
			throw new NotImplementedException();
		}

		private void ProcessUserInput(ReferenceHub hub, ServerSpecificSettingBase setting)
		{
			switch (setting.SettingId)
			{
				case (int)CustomSettingsManager.SettingsIDs.SCP_SwapToHuman:
					{
						if (setting is SSKeybindSetting kbSetting && kbSetting.SyncIsPressed)
						{
							if (SwapManager.CanScpSwapToHuman(hub, out string reason))
								SwapManager.SwapScpToHuman(hub);
							else
								hub.gameConsoleTransmission.SendToClient(reason, "green");

						}
						break;
					}

				case (int)CustomSettingsManager.SettingsIDs.SCP_SwapFromHuman:
					{
						if (setting is SSKeybindSetting kbSetting && kbSetting.SyncIsPressed)
						{
							if (SwapManager.CanHumanSwapToScp(hub, out string reason))
								SwapManager.SwapHumanToScp(hub);
							else
								hub.gameConsoleTransmission.SendToClient(reason, "green");

						}

						break;
					}

				default:
					break;
			}
		}
	}
}
