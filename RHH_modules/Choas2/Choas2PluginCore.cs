using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using RedRightHandCore;
using RedRightHandCore.CustomEvents;
using RedRightHandMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choas2
{
	//To keep things neat and unified, the main class for plugins should always be named "<Name>PluginCore".
	//If they are a submodule of RedRightHand, they need to inherit the ModuleCore class, and NOT the Plugin class
    public class Choas2PluginCore : ModuleCore
    {
		public override LoadPriority Priority { get; } = LoadPriority.High;

		//For modules, we use ModuleName, and NOT Name
		public override string ModuleName => "Choas 2";

		public override string Description => "MEOW";

		public override string Author => "Dragon Inn Tech Team";

		public override Version Version => new Version(0, 0, 0, 2);

		public override void Disable()
		{
		}

		public override void Enable()
		{
			CustomEvents.InvokeLogToDiscord(new LogToDiscordEventArgs("MEOW MEOW MEOW MEOW"));
		}
	}
}
