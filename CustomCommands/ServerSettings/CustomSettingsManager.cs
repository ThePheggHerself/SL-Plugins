using CustomCommands.Features.SCPs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserSettings.ServerSpecific;

namespace CustomCommands.ServerSettings
{
	public static class CustomSettingsManager
	{
		public enum SettingsIDs
		{
			SCP_SwapToHuman = 0,
			SCP_SwapFromHuman = 1,
			SCP_NeverSCP = 2,
			SCP_ZombieSuicide = 3,
			Human_HealOther = 4,
			Human_Suicide = 5,
		}

		public static readonly CustomSettingsBase[] CustomSettings =
		{
			new CustomSCPSettings()
		};

		public static ServerSpecificSettingBase[] GetAllSettings(bool deactivate = false)
		{
			List<ServerSpecificSettingBase> ssSettingBases = new List<ServerSpecificSettingBase>();

			foreach (var customSettings in CustomSettings)
			{
				if (deactivate)
					customSettings.Deactivate();
				else
					customSettings.Activate();

				ssSettingBases.AddRange(customSettings.SettingBases);
			}

			return ssSettingBases.ToArray();
		}
	}
}
