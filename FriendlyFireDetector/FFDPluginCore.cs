using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Loader;
using RedRightHandCore;
using RedRightHandCore.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendlyFireDetector
{
	public class FFDPluginCore : CustomPluginCore<FFDConfig>
	{
		public override string Name => "Friendly Fire Detector";

		public override string Description => "Anti-Friendly Fire system";

		public override string Author => "Dragon Inn Tech Team";

		public override Version Version => new(0, 0, 0, 1);

		public Events Events { get; } = new Events();

		public override string ConfigFileName => "FFDConfig.yml";

		public override void Enable()
		{
			base.Enable();

			CustomHandlersManager.RegisterEventsHandler(Events);
		}

		public override void Disable()
		{
			CustomHandlersManager.UnregisterEventsHandler(Events);
		}
	}
}
