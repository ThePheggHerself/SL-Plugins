using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Loader;
using RedRightHandCore;
using RedRightHandCore.CustomEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendlyFireDetector
{
	public class FFDPluginCore : ModuleCore
	{
		public static FFDConfig Config;
		private bool _correctConfigLoaded = false;
		public override string ModuleName => "Friendly Fire Detector";

		public override string Description => "Anti-Friendly Fire system";

		public override string Author => "Dragon Inn Tech Team";

		public override Version Version => new Version(0, 0, 0, 1);

		public Events Events { get; } = new Events();

		public override void LoadConfigs()
		{
			base.LoadConfigs();

			Logger.Info($"Loading Config");

			_correctConfigLoaded = this.TryLoadConfig("FFDConfig.yml", out Config);
		}



		public override void Enable()
		{
			if (!_correctConfigLoaded)
			{
				Logger.Error($"Configs have not loaded correctly. Please make sure the configs are correctly set. This plugin will NOT be enabled");
				return;
			}

			CustomHandlersManager.RegisterEventsHandler(Events);
		}

		public override void Disable()
		{
			CustomHandlersManager.UnregisterEventsHandler(Events);
		}
	}
}
