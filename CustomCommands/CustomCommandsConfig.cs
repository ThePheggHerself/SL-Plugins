using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands
{
	public class CustomCommandsConfig : ConfigBase
	{
		[YamlDotNet.Serialization.YamlMember]
		public bool DebugMode = false;

		//Enables the auto creation of a dummy called Steve
		public bool EnableSteve = false;
	}
}
