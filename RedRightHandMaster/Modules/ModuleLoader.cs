using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Plugins;
using RedRightHandCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedRightHandMaster.Modules
{
	public class ModuleLoader
	{
		private const string DllSearchPattern = "*.dll";
		private const string ModuleDir = "rrh_modules";

		public static Dictionary<Plugin, Assembly> Modules = [];

		public ModuleLoader()
		{
			var path = PathManager.Plugins.GetDirectories(ModuleDir);

			Logger.Info(path.First());

			if (path.Any())
			{
				//Essentially directly cut and paste from the PluginLoader
				//Same logic as loading plugins. Search file x for plugins, then 
				Logger.Info($"Searching for modules");
				PluginLoader.LoadPlugins(path.First().GetFiles(DllSearchPattern));

				foreach (var p in PluginLoader.Plugins.OrderBy(p => p.Key.Properties))
				{
					Logger.Info($"Checking: {p.Key.Name} {p.Key.TryLoadProperties()} {PluginLoader.EnabledPlugins.Contains(p.Key)}");

					if (PluginLoader.EnabledPlugins.Contains(p.Key))
						continue;

					try
					{
						PluginLoader.EnablePlugin(p.Key);
					}
					catch (Exception e)
					{
						Logger.Error(e);
					}
				}
			}
			else
			{
				Logger.Info($"Module file not found");
				PathManager.Plugins.CreateSubdirectory(ModuleDir);
			}
		}
	}
}
