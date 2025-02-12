using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedRightHandCore
{
	public abstract class ModuleCore : Plugin
	{
		public abstract string ModuleName { get; }
		public override string Name => $"RRH - {ModuleName}";
		public override Version RequiredApiVersion => new Version(LabApiProperties.CompiledVersion);
	}
}
