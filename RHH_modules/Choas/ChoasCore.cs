using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using RedRightHandCore;
using RedRightHandCore.CustomEvents;
using RedRightHandMaster;
using RedRightHandMaster.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choas
{
	public class ChoasCore : ModuleCore
	{
		public override LoadPriority Priority { get; } = LoadPriority.High;
		public override string ModuleName => "Choas";

		public override string Description => "Random plugin for testing and causing chaos";

		public override string Author => "Dragon Inn Tech Team";

		public override Version Version => new Version(0,0,0,1);

		public override void Disable()
		{
			ServerEvents.WaitingForPlayers -= ServerEvents_WaitingForPlayers;
		}

		public override void Enable()
		{
			ServerEvents.WaitingForPlayers += ServerEvents_WaitingForPlayers;
		}

		public void ServerEvents_WaitingForPlayers()
		{
			Logger.Info("Testing custom event");

			CustomEvents.InvokeLogToDiscord(new LogToDiscordEventArgs("MEOW MEOW MEOW MEOW"));
		}
	}
}
