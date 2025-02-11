using LabApi.Events.CustomHandlers;
using RedRightHandCore.CustomEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedRightHandMaster.Modules
{
	public class ModuleEvents
	{
		public event EventHandler<LogToDiscordEventArgs> LogToDiscord;
	}
}
