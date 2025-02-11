using LabApi.Events.CustomHandlers;
using RedRightHandCore.CustomEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedRightHandMaster
{
	public class CustomEvents
	{
		public static void InvokeLogToDiscord(LogToDiscordEventArgs args)
		{
			LogToDiscord?.Invoke(args);
		}
		public static event Action<LogToDiscordEventArgs> LogToDiscord;	
	}
}
