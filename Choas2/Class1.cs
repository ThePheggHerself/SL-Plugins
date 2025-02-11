using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using RedRightHandCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choas2
{
    public class Class1 : Plugin
    {
		public override LoadPriority Priority { get; } = LoadPriority.High;
		public override string Name => "RHH - Choas 2";

		public override string Description => "MEOW";

		public override string Author => "Dragon Inn Tech Team";

		public override Version Version => new Version(0, 0, 0, 2);

		public override Version RequiredApiVersion => new Version(LabApiProperties.CompiledVersion);

		public override void Disable()
		{
			RedRightHandMaster.CustomEvents.LogToDiscord -= CustomEvents_LogToDiscord;
		}

		public override void Enable()
		{
			RedRightHandMaster.CustomEvents.LogToDiscord += CustomEvents_LogToDiscord;
		}

		private void CustomEvents_LogToDiscord(RedRightHandCore.CustomEvents.LogToDiscordEventArgs obj)
		{
			Logger.Info(obj.LogString);
		}
	}
}
