using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedRightHandCore.CustomEvents
{
	public class LogToDiscordEventArgs : CustomEventArgs
	{
		public string LogString {  get; set; }
		public LogToDiscordEventArgs(string logString)
		{
			LogString = logString;
		}
	}
}
