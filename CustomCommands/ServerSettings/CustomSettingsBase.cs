using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserSettings.ServerSpecific.Examples;
using UserSettings.ServerSpecific;
using CustomCommands.Features.SCPs.Settings;

namespace CustomCommands.ServerSettings
{
	public abstract class CustomSettingsBase
	{
		private static CustomSettingsBase _instance;

		/// <summary>
		/// Name
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Quick description of the setting
		/// </summary>
		public abstract string Description { get; }

		public abstract ServerSpecificSettingBase[] SettingBases { get; }

		public abstract void Activate();
		public abstract void Deactivate();
	}
}
