using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using MEC;
using RedRightHandMaster.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedRightHandMaster
{
	public class MasterCore : Plugin
	{
		public ModuleLoader ModuleLoader { get; private set; }
		public override LoadPriority Priority { get; } = LoadPriority.Lowest;

		public override string Name => "Red Right Hand Master";

		public override string Description => "Master system for the RedRightHand plugin";

		public override string Author => "PheWitch";

		public override Version Version => new (0, 0, 0, 1);

		public override Version RequiredApiVersion { get; } = new (LabApiProperties.CompiledVersion);

		public override void Enable()
		{
			Logger.Info("Hello, World!");
			Properties.IsEnabled = true;

			//Wait for every other plugin to have finished loading
			Timing.CallDelayed(0.5f, () =>
			{
				ModuleLoader = new ModuleLoader();
			});
			

			
		}

		public override void Disable()
		{
			Logger.Info("Goodbye, World!");
		}
	}
}
