using LabApi.Features.Console;
using LabApi.Loader;
using RedRightHandCore;
using RedRightHandCore.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands
{
	public class CustomCommandsPluginCore : CustomPluginCore
	{
		public static CustomCommandsConfig Config;
		private bool _correctConfigLoaded = false;
		public override string Name => "Shenanigans";

		public override string Description => "Plugin filled with various moderation and fun commands and features";

		public override string Author => "Dragon Inn Tech Team";

		public override Version Version => new Version(0, 0, 0, 1);

		public override void LoadConfigs()
		{
			base.LoadConfigs();

			Logger.Info($"Loading Config");

			_correctConfigLoaded = this.TryLoadConfig("ShenanigansConfig.yml", out Config);
		}


		public override void Enable()
		{
			if (!_correctConfigLoaded)
			{
				Logger.Error($"Configs have not loaded correctly. Please make sure the configs are correctly set. This plugin will NOT be enabled");
				return;
			}
		}

		public override void Disable()
		{
			throw new NotImplementedException();
		}
	}
}
