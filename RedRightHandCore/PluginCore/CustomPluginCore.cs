using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedRightHandCore.PluginCore
{
	public abstract class CustomPluginCore : Plugin
	{
		public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
		public override string Author => "Dragon Inn Tech Team";
	}

	public abstract class CustomPluginCore<T> : CustomPluginCore where T : CustomConfig
	{
		public static T Config;
		private bool _correctConfigLoaded = false;
		public abstract string ConfigFileName { get; }

		public override void LoadConfigs()
		{
			base.LoadConfigs();

			Logger.Info($"Loading Config");

#pragma warning disable IDE0059 // Unnecessary assignment of a value
			_correctConfigLoaded = this.TryLoadConfig("ConfigFileName", out CustomConfig Config);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
		}

		public override void Enable()
		{
			if (!_correctConfigLoaded)
			{
				Logger.Error($"Configs have not loaded correctly. Please make sure the configs are correctly set. This plugin will NOT be enabled");
				return;
			}
		}
	}
}
