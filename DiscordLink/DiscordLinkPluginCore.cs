using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Loader;
using RedRightHandCore;
using RedRightHandCore.PluginCore;
using System;

namespace DiscordLink
{
	public class DiscordLinkPluginCore : CustomPluginCore<DiscordLinkConfig>
	{
		public override string Name => "DiscordLink";

		public override string Description => "Handles linking your server to discord for logging";

		public override Version Version => new(0, 0, 0, 1);

		public Events Events { get; } = new Events();

		public override string ConfigFileName => "DiscordLabConfig.yml";


		public override void Enable()
		{
			base.Enable();
			
			Logger.Info($"Discord link enabled. Link address for bot: {Config.BotAddress}:{Config.BotPort}");
			new BotLink();

			CustomHandlersManager.RegisterEventsHandler(Events);

			BotLink.AddLog += BotLink.Instance.LogAddedByPlugin;
		}



		public override void Disable()
		{
			CustomHandlersManager.UnregisterEventsHandler(Events);

			BotLink.AddLog -= BotLink.Instance.LogAddedByPlugin;
		}
	}
}
