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

namespace DiscordLink
{
	public class DiscordLinkPluginCore : ModuleCore
	{
		public static DiscordLinkConfig Config;
		private bool _correctConfigLoaded = false;

		public override string ModuleName => "DiscordLink";

		public override string Description => "Handles linking your server to discord for logging";

		public override string Author => "Dragon Inn Tech Team";

		public override Version Version => new Version(0, 0, 0, 1);

		public Events Events { get; } = new Events();

		public override void LoadConfigs()
		{
			base.LoadConfigs();

			Logger.Info($"Loading Config");

			_correctConfigLoaded = this.TryLoadConfig("DiscordLabConfig.yml", out Config);
		}


		public override void Enable()
		{
			if (!_correctConfigLoaded)
			{
				Logger.Error($"Configs have not loaded correctly. Please make sure the configs are correctly set. This plugin will NOT be enabled");
				return;
			}
			
			Logger.Info($"Discord link enabled. Link address for bot: {Config.BotAddress}:{Config.BotPort}");
			new BotLink();

			CustomHandlersManager.RegisterEventsHandler(Events);

			RedRightHandMaster.CustomEvents.LogToDiscord += LogToDiscord;
		}

		private void LogToDiscord(LogToDiscordEventArgs obj)
		{
			Logger.Info($"Incoming message from plugins: {obj.LogString}");
			BotLink.Instance.SendMessage(obj.LogString);
		}

		public override void Disable()
		{
			CustomHandlersManager.UnregisterEventsHandler(Events);

			RedRightHandMaster.CustomEvents.LogToDiscord -= LogToDiscord;
		}
	}
}
